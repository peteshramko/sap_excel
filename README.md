# sap_excel
this product is covered under the GNU Public license

Skeleton SAP to Excel integration with a complete end to end SAP Post example, RFC_READ_DATA example to follow soon!

This is an open source effort to capture as much end-to-end SAP to excel functionality as possible.

The *basic* architecture:

1 - A VB.NET COM wrapper for the RTD interface (see: Note 1)
2 - C#
  a - implementation of the COM RTD initial excel data capture
  b - implementation of data throttle / queue as we pass query or post data to SAP
  c - .NET sapco RFC interface to SAP uploads data to SAP server
  d - return data is then parsed back into excel
  

 We will use dialog with progress bar to show detail report progress as data moves back and for from excel. Most of the code will
 be C# with noted exception above. I have included a reference to the XLL tools that could allow native code as a link-layer rather
 than VBA scripting - this has not been implemented.
 
 Dialog boxes can also write back functions to excel cells, and example is coming for that as well.
 
 You may modify, tinker and add to this project. It is completely open source (GNU Public license).
 
 
 
 
 Note 1: It is possible to hard code C# to COM using IUNKNOWN interface. This is, sadly, not optimal. 
         Behavior is erratic at best.
         The compromise of using a VB.NET RTD COM wrapper (capable of late-binding by default) comes after much testing 
         and research. It works quite well. The handoff of data to C# dll as RTD kicks in is smooth. The readback uses
         a event override for Dictionary update in C# which VB picks up on the down stream side.
