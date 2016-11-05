!if "$(TYPE)" == ""
TYPE=DEBUG
!endif

SDKDIR  =   ..\..
MATMANXLLADDINDIR=	.
!if "$(PLATFORM)" == "Win32" || "$(PLATFORM)" == "x86" || "$(PLATFORM)" == ""
MATMANXLLADDINBUILDDIR = $(MATMANXLLADDINDIR)\$(TYPE)
PREVDIR = .\..
TARGETLIBDIR = $(PREVDIR)\$(SDKDIR)\lib
DLLDEF_FLAGS = -def:$(PREVDIR)\MatManXLLAddIn.def
!else
MATMANXLLADDINBUILDDIR = $(MATMANXLLADDINDIR)\$(PLATFORM)\$(TYPE)
PREVDIR = .\..\..
TARGETLIBDIR = $(PREVDIR)\$(SDKDIR)\lib\$(PLATFORM)
DLLDEF_FLAGS =
!endif
TARGETINCDIR = $(SDKDIR)\include
TARGETSRCDIR = $(SDKDIR)\src

PROJ = MatManXLLAddIn

COPY=copy /Y

!if "$(RM)" == ""
RM=-del /f /q
!endif

!if "$(RMDIR)" == ""
RMDIR=-rd /S /Q
!endif

all : TARGET_DELIVER

MATMANXLLADDIN_BINARY   = $(PROJ).xll
MATMANXLLADDIN_PDB	 = $(MATMANXLLADDINBUILDDIR)\$(PROJ).pdb  
MATMANXLLADDIN_INCLUDE  = /I"$(MATMANXLLADDINDIR)" /I"$(TARGETINCDIR)" /I"$(TARGETSRCDIR)"

!if "$(TYPE)" == "DEBUG"
MATMANXLLADDIN_LIBS = msvcrtd.lib  kernel32.lib user32.lib gdi32.lib winspool.lib comdlg32.lib advapi32.lib shell32.lib ole32.lib oleaut32.lib uuid.lib odbc32.lib odbccp32.lib $(TARGETLIBDIR)\Frmwrk32.lib $(TARGETLIBDIR)\xlcall32.lib
!else
MATMANXLLADDIN_LIBS = msvcrt.lib  kernel32.lib user32.lib gdi32.lib winspool.lib comdlg32.lib advapi32.lib shell32.lib ole32.lib oleaut32.lib uuid.lib odbc32.lib odbccp32.lib $(TARGETLIBDIR)\Frmwrk32.lib $(TARGETLIBDIR)\xlcall32.lib
!endif

!if "$(TYPE)" == "DEBUG"
MATMANXLLADDIN_LIB_OPTS = /OUT:$(MATMANXLLADDIN_BINARY)  /subsystem:windows $(DLLDEF_FLAGS) /DEBUG /NODEFAULTLIB:msvcrt.lib /INCREMENTAL:NO
!else
MATMANXLLADDIN_LIB_OPTS = /OUT:$(MATMANXLLADDIN_BINARY)  /subsystem:windows $(DLLDEF_FLAGS) /RELEASE /NODEFAULTLIB:msvcrtd.lib /OPT:NOICF
!endif

!if "$(TYPE)" == "DEBUG"
MATMANXLLADDIN_DEFINES  = /D"WIN32" /D"_WINDOWS" /D"_MBCS" /D"_USRDLL" /D"MATMANXLLADDIN_EXPORTS" /D "_DEBUG" 
!else
MATMANXLLADDIN_DEFINES  = /D"WIN32" /D"_WINDOWS" /D"_MBCS" /D"_USRDLL" /D"MATMANXLLADDIN_EXPORTS" /D "NDEBUG"
!endif

MATMANXLLADDIN_OBJ =	MATMANXLLADDIN.obj

.SUFFIXES:	.cpp .c .obj

!if "$(TYPE)" == "DEBUG"
CPPFLAGS        =/Od /W3 /WX /EHsc /Zi /MDd /Fd"$(MATMANXLLADDIN_PDB)" /Fo"$(MATMANXLLADDINBUILDDIR)\\"
!else
CPPFLAGS        =/W3 /WX /EHsc /MD /Fo"$(MATMANXLLADDINBUILDDIR)\\"
!endif

minclean:
	$(RM) "$(MATMANXLLADDINBUILDDIR)\*.obj" 1>nul 2>nul
	$(RM) "$(MATMANXLLADDINBUILDDIR)\*.lib" 1>nul 2>nul
	$(RM) "$(MATMANXLLADDINBUILDDIR)\*.exp" 1>nul 2>nul
	$(RM) "$(MATMANXLLADDINBUILDDIR)\*.manifest" 1>nul 2>nul
!if "$(TYPE)" == "DEBUG"
	$(RM) "$(MATMANXLLADDINBUILDDIR)\*.pdb" 1>nul 2>nul
!endif

clean: 
	$(RM) "$(MATMANXLLADDINBUILDDIR)\*.*" 1>nul 2>nul
	$(RMDIR) $(MATMANXLLADDINBUILDDIR) 1>nul 2>nul
	
"$(MATMANXLLADDINBUILDDIR)":
	if not exist "$(MATMANXLLADDINBUILDDIR)" mkdir "$(MATMANXLLADDINBUILDDIR)"

TARGET_DELIVER: "$(MATMANXLLADDINBUILDDIR)" $(MATMANXLLADDIN_OBJ)
	cd $(MATMANXLLADDINBUILDDIR)
	link.exe /DLL /manifest /nologo $(MATMANXLLADDIN_OBJ) $(MATMANXLLADDIN_LIB_OPTS) $(MATMANXLLADDIN_LIBS)
	mt.exe /nologo /outputresource:"$(MATMANXLLADDIN_BINARY);#2" /manifest "$(MATMANXLLADDIN_BINARY).manifest"
	
.cpp.obj:
	$(CPP) /nologo $(CPPFLAGS) $(MATMANXLLADDIN_INCLUDE) $(MATMANXLLADDIN_DEFINES) /c $<

.c.obj:
	$(CPP) /nologo $(CPPFLAGS) $(MATMANXLLADDIN_INCLUDE) $(MATMANXLLADDIN_DEFINES) /c $<
