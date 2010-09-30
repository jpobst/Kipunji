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
using Kipunji.Models;

namespace Kipunji.Adapters
{
	public class MdocAdapter : BaseAdapter
	{
		private string doc_dir;
		private List<AssemblyModel> index;

		public override void Initialize (string docDirectory)
		{
			doc_dir = docDirectory;
			CreateIndex ();
		}

		public override List<AssemblyModel> GetIndex ()
		{
			return index;
		}

		private void CreateIndex ()
		{
			index = new List<AssemblyModel> ();

			foreach (string dir in Directory.GetDirectories (doc_dir)) {
				string index_file = Path.Combine (dir, "index.xml");

				if (!File.Exists (index_file))
					continue;

				XmlDocument doc = new XmlDocument ();
				doc.Load (index_file);

				AssemblyModel am = new AssemblyModel ();

				am.Name = GetChildText (doc.DocumentElement, "Title", "index.xml does not have <Title>") + ".dll";
				am.Remarks = GetChildText (doc.DocumentElement, "Remarks", "");

				foreach (XmlElement xe in doc.DocumentElement.SelectNodes ("Types/Namespace")) {
					NamespaceModel ns = new NamespaceModel ();

					ns.Assembly = am.Name;
					ns.Name = xe.GetAttribute ("Name");

					foreach (XmlElement t in xe.SelectNodes ("Type")) {
						TypeModel tm = new TypeModel ();
						tm.Assembly = am.Name;
						tm.Name = t.GetAttribute ("Name");

						ns.Types.Add (tm);
					}

					am.Namespaces.Add (ns);
				}

				index.Add (am);
			}
		}

		// Read in the information needed for NamespaceModel
		// - Namespace overview + shallow list of Types
		public override NamespaceModel ReadNamespace (string assembly, string name)
		{
			string filename = string.Format ("ns-{0}.xml", name);
			string file = Path.Combine (Path.Combine (doc_dir, assembly), filename);

			XmlDocument doc = new XmlDocument ();
			doc.Load (file);

			NamespaceModel model = new NamespaceModel ();
			XmlElement xe = doc.DocumentElement;

			model.Assembly = assembly;
			model.Name = xe.GetAttribute ("Name");

			XmlElement docs = xe["Docs"];

			if (docs != null) {
				model.Summary = GetChildXml (docs, "summary", string.Empty);
				model.Remarks = GetChildXml (docs, "remarks", string.Empty);
			}

			PopulateTypesInNamespace (assembly, model);

			return model;
		}

		// Read the information need for TypeModel
		// If !shallow, read in information for all Members
		public override TypeModel ReadType (string assembly, string ns, string type, bool shallow)
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
			model.Name = xe.GetAttribute ("Name");
			model.BaseType = GetChildXml (xe, "Base", string.Empty);

			// Populate the type signature
			XmlElement sig = (XmlElement)xe.SelectSingleNode ("TypeSignature[@Language='C#']");

			if (sig != null) {
				model.Signature = new Signature (sig);
				model.Kind = model.Signature.TypeKind;
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
				model.Summary = GetChildXml (docs, "summary", string.Empty);
				model.Remarks = GetChildXml (docs, "remarks", string.Empty);
			}

			// Populate the AssemblyInfo
			XmlElement assem = xe["AssemblyInfo"];

			if (assem != null)
				model.AssemblyInfo = new AssemblyInfo (assem);

			if (!shallow)
				PopulateMembersInType (model, xe);

			return model;
		}

		// Read the information need for MemberModel
		public override MemberModel ReadMember (string assembly, string ns, string type, string member)
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
			model.ParentType = type;
			model.Type = GetChildXml (xe, "MemberType", string.Empty);
			model.ReturnType = GetChildText (xe, "ReturnValue", string.Empty);
			model.ReturnSummary = GetChildXml (xe, "returns", string.Empty);

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
				model.Summary = GetChildXml (docs, "summary", string.Empty);
				model.Remarks = GetChildXml (docs, "remarks", string.Empty);
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

			model.Types.Clear ();

			XmlElement name_space = (XmlElement)doc.SelectSingleNode (string.Format ("Overview/Types/Namespace[@Name='{0}']", model.Name));

			if (name_space == null)
				return;

			foreach (XmlElement xe in name_space.ChildNodes) {
				TypeModel t = new TypeModel ();

				t.Assembly = assembly;
				t.Namespace = model.Name;
				t.Name = xe.GetAttribute ("Name");
				t.Kind = xe.GetAttribute ("Kind");
				t.Summary = GetChildXml (xe, "summary", string.Empty);
				
				model.Types.Add (t);
			}
		}

		private void PopulateMembersInType (TypeModel model, XmlElement xe)
		{
			model.Members.Clear ();

			string type_name = xe.GetAttribute ("Name");

			foreach (XmlElement x in xe["Members"].ChildNodes) {
				MemberModel m = new MemberModel ();

				m.Assembly = model.Assembly;
				m.Namespace = model.Namespace;
				m.Name = x.GetAttribute ("MemberName");
				m.Type = GetChildXml (x, "MemberType", string.Empty);
				m.ParentType = model.Name;
				m.ReturnType = GetChildText (x, "ReturnValue", string.Empty);
				m.ReturnSummary = GetChildXml (x, "returns", string.Empty);

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
					m.Summary = GetChildXml (docs, "summary", string.Empty);
					m.Remarks = GetChildXml (docs, "remarks", string.Empty);
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

			return x.InnerXml;
		}

		private static string GetChildText (XmlElement xe, string child, string defaultValue)
		{
			XmlElement x = xe[child];

			if (x == null)
				return defaultValue;

			return x.InnerText;
		}
	}
}