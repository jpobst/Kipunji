Kipunji
=======

Kipunji is a simple, modern ASP.NET MVC web frontend for viewing API reference documentation of .Net libraries.

**Live demo:** [http://kipunji.jpobst.com/](http://kipunji.jpobst.com/)

The Main Features of Kipunji:
-----------------------------

 - **Attractive, Light, and Quick:** Navigating documentation can be frustrating when it takes each page several seconds to load.  Kipunji documentation is modern looking, extremely lightweight, and loads instantly. 

 - **Simple URLs:** The URL for your documentation is exactly what you would think it is.  Kipunji will figure out if you wanted a namespace, type, method, etc.:

	* http://mydocs.example.com/System
	* http://mydocs.example.com/System.String
	* http://mydocs.example.com/System.String.Trim()


 - **No Intermediate Generation:**  Many documentation systems have an intermediate step where the XML documentation is converted to static HTML.  Kipunji avoids this by operating directly on the XML.

 - **Online Editing:** Because Kipunji operates directly on your XML documentation instead of a static HTML copy, online editing is trivial.  **(Not implemented yet)**

Status of Kipunji
-----------------

- **Basics:** Kipunji supports basic viewing of documentation.  Some advanced cases like generics, interfaces, and nested classes are not implemented.  Also there are likely bugs in corner cases.

- **XML Formats:** Currently the only supported format is [MDoc](http://www.mono-project.com/Mdoc).  Supporting a new format is rather simple, the entire code necessary for MDoc support is available [here](http://github.com/jpobst/Kipunji/blob/master/Kipunji/Adapters/MdocAdapter.cs).

- **Online Editing:** Online editing is not yet supported, but should be relatively easy to add.

Similar Projects
----------------

Some other similar projects:

- [MDoc](http://www.mono-project.com/Mdoc)
- [SandCastle](http://sandcastle.codeplex.com/)