//
// Authors:
// Jonathan Pobst (monkey@jpobst.com)
//
// Copyright (C) 2010 Jonathan Pobst
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Linq;
using Kipunji.Models;
using System.Threading;

namespace Kipunji.Adapters
{
	public class MdocAdapter : BaseAdapter
	{
		private static readonly string RESOURCES_DIR = "Resources";
		private static readonly string EDITORS_FILENAME = "editors";
		private static readonly string ECMA_TRANSFORM_FILENAME = "mono-ecma-css.xsl";
		
		private static AutoResetEvent edit_event = new AutoResetEvent (true);
		
		private string doc_dir;
		private string resources_dir;
		private List<string> editors;
		private List<NamespaceModel> index;
		private List<NamespaceModel> display_index;
		

		public override void Initialize (string docDirectory)
		{
			doc_dir = docDirectory;
			CreateIndex ();
			CreateEditorsList ();
		}

		public override List<NamespaceModel> GetIndex ()
		{
			return index;
		}

		public override List<NamespaceModel> GetDisplayIndex ()
		{
			return display_index;
		}
		
		public override string GetEcmaTransformXslPath ()
		{
			return Path.Combine (RESOURCES_DIR, ECMA_TRANSFORM_FILENAME);
		}
		
		public override bool IsEditor (string email)
		{
			return editors.Contains (email);
		}
		
		public override TypeModel GetType (string ns, string type)
		{
			NamespaceModel nsmod = index.Where (n => n.Name == ns).FirstOrDefault ();
			
			if (nsmod == null)
				return null;
			
			return nsmod.Types.Where (t => t.Name == type).FirstOrDefault ();
		}
		
		public override MemberModel GetMember (string ns, string type, string member)
		{
			TypeModel typemod = GetType (ns, type);
			
			if (typemod == null)
				return null;
			
			return typemod.Members.Where (m => m.Signature.Value == member).FirstOrDefault ();
		}
		
		private void CreateEditorsList ()
		{
			editors = new List<string> ();
			
			string editors_file = Path.Combine (RESOURCES_DIR, EDITORS_FILENAME);
			if (!File.Exists (editors_file)) {
				Console.Error.WriteLine ("No editors file found. Users will not be allowed to edit data.");
				return;
			}
		
			using (StreamReader reader = File.OpenText (editors_file)) {
				string line;
				while ((line = reader.ReadLine ()) != null) {
					string n = line.Trim ();
					if (String.IsNullOrEmpty (n) || n.StartsWith ("#"))
						continue;
					editors.Add (line.Trim ());	
				}
			}
		}
		
		public override AutoResetEvent BeginEdit (string log)
		{
			edit_event.WaitOne ();
			
			using (StreamWriter writer = File.AppendText (Path.Combine (doc_dir, "editlog"))) {
				writer.WriteLine (log);
			}
			return edit_event;
		}
		
		private void CreateIndex ()
		{
			index = new List<NamespaceModel> ();
			display_index = new List<NamespaceModel> ();

			foreach (string dir in Directory.GetDirectories (doc_dir)) {
				string index_file = Path.Combine (dir, "index.xml");

				if (!File.Exists (index_file))
					continue;

				XmlDocument doc = new XmlDocument ();
				doc.Load (index_file);

				AssemblyModel am = new AssemblyModel ();

				am.Name = GetChildText (doc.DocumentElement, "Title", "index.xml does not have <Title>") + ".dll";
				am.Remarks = GetChildXml (doc.DocumentElement, "Remarks", "");

				foreach (XmlElement xe in doc.DocumentElement.SelectNodes ("Types/Namespace")) {
					NamespaceModel ns = ReadNamespace (Path.GetFileName (dir), xe.GetAttribute ("Name"));

					if (String.IsNullOrEmpty (xe.GetAttribute ("Name")))
						continue;
					
					if (ns == null)
						continue;
					
					ns.Assembly = am.Name;
					index.Add (ns);
				}
			}
			
			index.Sort ((l, r) => String.Compare (l.Name, r.Name));
			display_index.Sort ((l, r) => String.Compare (l.Name, r.Name));
		}
		
		// Read in the information needed for NamespaceModel
		// - Namespace overview + shallow list of Types
		public NamespaceModel ReadNamespace (string assembly, string name)
		{
			string filename = string.Format ("ns-{0}.xml", name);
			string file = Path.Combine (Path.Combine (doc_dir, assembly), filename);

			if (!File.Exists (file))
				return null;
			
			XmlDocument doc = new XmlDocument ();
			doc.Load (file);
			
			XmlElement xe = doc.DocumentElement;
			NamespaceModel model = index.Where (ns => ns.Name == name).FirstOrDefault ();
			bool created = false;
			if (model == null) {
				model = new NamespaceModel ();
				model.Assembly = assembly;
				model.Name = xe.GetAttribute ("Name");
				created = true;
				if (!name.StartsWith ("Microsoft.") && !name.StartsWith ("System.") && name != "System"){
				    display_index.Add (model);
  			    }
			}

			XmlElement docs = xe["Docs"];

			if (docs != null) {
				if (created || !model.HasSummary)
					model.Summary = GetChildSummaryOrRemarksXml (docs, "summary", string.Empty);
				if (created || !model.HasRemarks)
					model.Remarks = GetChildSummaryOrRemarksXml (docs, "remarks", string.Empty);
			}

			PopulateTypesInNamespace (assembly, model);

			if (!created)
				return null;
			
			return model;
		}

		// Read the information need for TypeModel
		// If !shallow, read in information for all Members
		public TypeModel ReadType (string assembly, string ns, string type, bool shallow)
		{
			string path = Path.Combine (Path.Combine (doc_dir, assembly), ns);
			string filename = string.Format ("{0}.xml", type);
			string file = Path.Combine (path, filename);

			XmlDocument doc = new XmlDocument ();
			doc.Load (file);

			XmlElement xe = doc.DocumentElement;
			TypeModel model = new TypeModel ();

			model.Assembly = assembly;
			model.Namespace = ns;
			model.Name = type; 
			model.DisplayName = xe.GetAttribute ("Name");
			
			if (xe ["Base"] != null)
				model.BaseType = GetChildText (xe ["Base"] as XmlElement, "BaseTypeName", string.Empty);

			// Populate the type signature
			XmlElement sig = (XmlElement)xe.SelectSingleNode ("TypeSignature[@Language='C#']");

			if (sig != null) {
				model.Signature = new Signature (sig);
				string sig_kind = model.Signature.TypeKind;
				if (sig_kind != null)
					model.Kind = sig_kind;
				model.Visibility = model.Signature.Visibility;
			}

			// Populate any interfaces the type implements
			XmlElement interfaces = xe["Interfaces"];

			if (interfaces != null)
				foreach (XmlElement p in interfaces.ChildNodes)
					model.Interfaces.Add (p.InnerText);

			// Populate the documentation
			XmlElement docs = xe["Docs"];

			if (docs != null) {
				model.Summary = GetChildSummaryOrRemarksXml (docs, "summary", string.Empty);
				model.Remarks = GetChildSummaryOrRemarksXml (docs, "remarks", string.Empty);
			}

			// Populate the AssemblyInfo
			XmlElement assem = xe["AssemblyInfo"];

			if (assem != null)
				model.AssemblyInfo = new AssemblyInfo (assem);

			if (!shallow && xe ["Members"] != null)
				PopulateMembersInType (model, xe);

			return model;
		}

		// Read the information need for MemberModel
		public MemberModel ReadMember (TypeModel parent_type, string assembly, string ns, string type, string member)
		{
			string path = Path.Combine (doc_dir, ns);
			string filename = string.Format ("{0}.xml", type);
			string file = Path.Combine (path, filename);

			XmlDocument doc = new XmlDocument ();
			doc.Load (file);

			XmlElement xe = (XmlElement)doc.SelectSingleNode (string.Format ("Type/Members/Member[@MemberName = '{0}']", member));
			MemberModel model = new MemberModel ();

			model.Namespace = ns;
			model.Name = xe.GetAttribute ("MemberName");
			model.Type = GetChildText (xe, "MemberType", string.Empty);

			model.ReturnType = GetChildText (xe, "ReturnValue", string.Empty);
			model.ReturnSummary = GetChildSummaryOrRemarksXml (xe, "returns", string.Empty);

			// Populate the member signature
			XmlElement sig = (XmlElement)xe.SelectSingleNode ("MemberSignature[@Language='C#']");

			if (sig != null) {
				model.Signature = new Signature (sig);
				model.Visibility = model.Signature.Visibility;
			}

			// Populate the member parameters
			XmlElement parameters = xe["Parameters"];
			XmlElement docs = xe["Docs"];

			if (parameters != null)
				foreach (XmlElement p in parameters.ChildNodes)
					model.Parameters.Add (new Parameter (p, docs));

			// Populate the documentation
			if (docs != null) {
				model.Summary = GetChildSummaryOrRemarksXml (docs, "summary", string.Empty);
				model.Remarks = GetChildSummaryOrRemarksXml (docs, "remarks", string.Empty);
			}

			// Populate the AssemblyInfo
			XmlElement assem = xe["AssemblyInfo"];

			if (assem != null)
				model.AssemblyInfo = new AssemblyInfo (assem);

			return model;
		}

		private void PopulateTypesInNamespace (string assembly, NamespaceModel model)
		{
			string ns_file = Path.Combine (Path.Combine (doc_dir, assembly), "index.xml");

			XmlDocument doc = new XmlDocument ();
			doc.Load (ns_file);

			XmlElement name_space = (XmlElement)doc.SelectSingleNode (string.Format ("Overview/Types/Namespace[@Name='{0}']", model.Name));

			if (name_space == null)
				return;

			foreach (XmlElement xe in name_space.ChildNodes) {
				TypeModel t = ReadType (assembly, model.Name, xe.GetAttribute ("Name"), false);

				string name = xe.GetAttribute ("DisplayName");
				if (!String.IsNullOrEmpty (name))
					t.DisplayName = name;
				t.Name = xe.GetAttribute ("Name");

				if (model.Types.Where (m => m.Name == t.Name).FirstOrDefault () == null)
					model.Types.Add (t);
			}
		}

		private void PopulateMembersInType (TypeModel model, XmlElement xe)
		{
			model.Members.Clear ();

			string type_name = xe.GetAttribute ("Name");

			foreach (XmlNode n in xe["Members"].ChildNodes) {
				
				XmlElement x = n as XmlElement;
				if (x == null)
					continue;
				
				MemberModel m = new MemberModel ();

				m.Assembly = model.Assembly;
				m.Namespace = model.Namespace;
				m.Name = x.GetAttribute ("MemberName");
				m.Type = GetChildText (x, "MemberType", string.Empty);
				m.ParentType = model;
				XmlElement rv = (XmlElement) x.SelectSingleNode ("ReturnValue");
				if (rv != null)
					m.ReturnType = GetChildText (rv, "ReturnType", "");

				m.ReturnSummary = GetChildSummaryOrRemarksXml (x, "returns", string.Empty);
			
				// Populate the member signature
				XmlElement sig = (XmlElement)x.SelectSingleNode ("MemberSignature[@Language='C#']");

				if (sig != null) {
					m.Signature = new Signature (sig);
					m.Visibility = m.Signature.Visibility;
				}

				// Populate the member parameters
				XmlElement parameters = x["Parameters"];
				XmlElement docs = x["Docs"];

				if (parameters != null)
					foreach (XmlElement p in parameters.ChildNodes)
						m.Parameters.Add (new Parameter (p, docs));

				// Populate the documentation
				if (docs != null) {
					m.Summary = GetChildSummaryOrRemarksXml (docs, "summary", string.Empty);
					m.Remarks = GetChildSummaryOrRemarksXml (docs, "remarks", string.Empty);
				}

				// Populate the AssemblyInfo
				XmlElement assem = x["AssemblyInfo"];

				if (assem != null)
					m.AssemblyInfo = new AssemblyInfo (assem);

				model.Members.Add (m);
			}
		}

		private static string GetChildXml (XmlElement xe, string child, string defaultValue)
		{
			XmlElement x = xe[child];

			if (x == null)
				return defaultValue;

			return x.OuterXml;
		}
		
		private static string GetChildSummaryOrRemarksXml (XmlElement xe, string child, string defaultValue)
		{
			XmlElement x = xe [child];
			
			if (x == null)
				return null;
			
			string inner = x.InnerText.Trim ();
			if (inner == "To be added" || inner == "To be added.")
				return null;
			return inner;
		}

		private static string GetChildText (XmlElement xe, string child, string defaultValue)
		{
			XmlElement x = xe[child];

			if (x == null)
				return defaultValue;

			return x.InnerXml;
		}
	}
}