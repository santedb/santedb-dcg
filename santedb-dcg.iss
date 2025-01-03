; Script generated by the Inno Setup Script Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

#define MyAppName "SanteDB Disconnected Gateway"
#define MyAppPublisher "SanteDB Community"
#define MyAppURL "http://santesuite.org"
#ifndef MyAppVersion
#define MyAppVersion "3.0"
#endif
#define SanteDBSdkPath "..\santedb-sdk"      


#ifndef SignKey
#define SignKey "8185304d2f840a371d72a21d8780541bf9f0b5d2"
#endif 

#ifndef SignOpts
#define SignOpts ""
#endif 


[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{89E5D303-441E-4742-970A-C2C240C89BD2}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
;AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={pf64}\SanteSuite\SanteDB\DCG
DisableProgramGroupPage=no
LicenseFile=.\License.rtf
OutputDir=.\dist
OutputBaseFilename=santedb-dcg-{#MyAppVersion}
Compression=bzip
SolidCompression=yes
DefaultGroupName={#MyAppName}
WizardStyle=modern
SignedUninstaller=yes
SignTool=default /sha1 {#SignKey} {#SignOpts} /d $qSanteDB dCDR Gateway Server$q $f


[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: ".\bin\Release\*.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: ".\bin\Release\runtimes\*"; DestDir: "{app}\runtimes"; Flags: recursesubdirs ignoreversion
Source: ".\bin\Release\Applets\*.pak"; DestDir: "{app}\Applets"; Flags: ignoreversion
Source: ".\bin\Release\santedb-dcg.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: ".\bin\Release\santedb-dcg.exe.config"; DestDir: "{app}"; Flags: ignoreversion
Source: ".\installsupp\restart.bat"; DestDir: "{app}"; Flags: ignoreversion;
Source: ".\installsupp\VC_redist.x64.exe"; DestDir: "{tmp}"; Flags: dontcopy;
Source: ".\installsupp\netfx.exe"; DestDir: "{tmp}"; Flags: dontcopy;

[Icons]
Filename: "http://127.0.0.1:9200"; Name: "{group}\SanteDB\Disconnected Gateway Admin"; IconFilename: "{app}\santedb-dcg.exe"
Filename: "{app}\santedb-dcg.exe"; Parameters: "--restart"; Name: "{group}\SanteDB\Restart Disconnected Gateway";  IconFilename: "{app}\santedb-dcg.exe"
Filename: "http://127.0.0.1:9200"; Name: "{commondesktop}\Disconnected Gateway"; IconFilename: "{app}\santedb-dcg.exe"

[Run]
Filename: "{app}\santedb-dcg.exe";StatusMsg: "Installing Services..."; Parameters: "--install"; Description: "Installing Service"; Flags: runhidden
Filename: "net.exe";StatusMsg: "Starting Services..."; Parameters: "start sdb-dcg-default"; Description: "Start Gateway Service"; Flags: runhidden
Filename: "http://127.0.0.1:9200"; Description: "Configure the Disconnected Gateway"; Flags: postinstall shellexec
Filename: "c:\windows\system32\netsh.exe"; Parameters: "advfirewall firewall add rule name=""Disconnected Gateway TCP"" dir=in protocol=TCP localport=9200,12100 action=allow"; StatusMsg: "Configuring Firewall"; Flags: runhidden
Filename: "c:\windows\system32\netsh.exe"; Parameters: "advfirewall firewall add rule name=""Disconnected Gateway UDP"" dir=in protocol=UDP localport=11514 action=allow"; StatusMsg: "Configuring Firewall"; Flags: runhidden

[UninstallRun]
Filename: "net.exe";StatusMsg: "Stopping Services..."; Parameters: "stop sdb-dcg-default"; Flags: runhidden
Filename: "{app}\santedb-dcg.exe";StatusMsg: "Removing Configuration Data...";  Parameters: "--reset"; Flags: runhidden
Filename: "{app}\santedb-dcg.exe";StatusMsg: "Removing Services..."; Parameters: "--uninstall"; Flags: runhidden

[Code]
function PrepareToInstall(var needsRestart:Boolean): String;
var
  hWnd: Integer;
  ResultCode : integer;
  uninstallString : string;
begin
    EnableFsRedirection(true);
    WizardForm.PreparingLabel.Visible := True;
    WizardForm.PreparingLabel.Caption := 'Installing Visual C++ Redistributable';
    ExtractTemporaryFile('VC_redist.x64.exe');
    Exec(ExpandConstant('{tmp}\VC_redist.x64.exe'), '/install /passive', '', SW_SHOW, ewWaitUntilTerminated, ResultCode);
    WizardForm.PreparingLabel.Caption := 'Installing Microsoft .NET Framework 4.8';
     ExtractTemporaryFile('netfx.exe');
    Exec(ExpandConstant('{tmp}\netfx.exe'), '/q', '', SW_SHOW, ewWaitUntilTerminated, ResultCode);
end;
