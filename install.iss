; Script generated by the Inno Setup Script Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

#define MyAppName "SanteDB Disconnected Gateway"
#define MyAppPublisher "SanteDB Community"
#define MyAppURL "http://santesuite.org"
#define MyAppVersion "2.0.59"
#define SanteDBSdkPath "..\santedb-sdk"      

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
;SignedUninstaller=yes
;SignTool=default

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: ".\bin\SignedRelease\Antlr3.Runtime.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: ".\bin\SignedRelease\AtnaApi.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: ".\bin\SignedRelease\ExpressionEvaluator.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: ".\bin\SignedRelease\Jint.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: ".\bin\SignedRelease\libcrypto-1_1.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: ".\bin\SignedRelease\spellfix.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: ".\bin\SignedRelease\esprima.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: ".\bin\SignedRelease\MohawkCollege.Util.Console.Parameters.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: ".\bin\SignedRelease\Mono.Data.Sqlite.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: ".\bin\SignedRelease\Newtonsoft.Json.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: ".\bin\SignedRelease\NHapi.Base.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: ".\bin\SignedRelease\NHapi.Model.V25.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: ".\bin\SignedRelease\RestSrvr.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: ".\bin\SignedRelease\SanteDB.BI.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: ".\bin\SignedRelease\SanteDB.BusinessRules.JavaScript.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: ".\bin\SignedRelease\SanteDB.Cdss.Xml.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: ".\bin\SignedRelease\SanteDB.Configuration.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: ".\bin\SignedRelease\SanteDB.Core.Api.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: ".\bin\SignedRelease\SanteDB.Core.Applets.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: ".\bin\SignedRelease\SanteDB.Core.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: ".\bin\SignedRelease\SanteDB.Core.Model.AMI.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: ".\bin\SignedRelease\SanteDB.Core.Model.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: ".\bin\SignedRelease\SanteDB.Core.Model.RISI.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: ".\bin\SignedRelease\SanteDB.Core.Model.ViewModelSerializers.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: ".\bin\SignedRelease\SanteDB.DisconnectedClient.Ags.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: ".\bin\SignedRelease\SanteDB.DisconnectedClient.Core.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: ".\bin\SignedRelease\SanteDB.DisconnectedClient.i18n.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: ".\bin\SignedRelease\SanteDB.DisconnectedClient.SQLite.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: ".\bin\SignedRelease\SanteDB.DisconnectedClient.UI.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: ".\bin\SignedRelease\SanteDB.Messaging.AMI.Client.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: ".\bin\SignedRelease\SanteDB.Messaging.HDSI.Client.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: ".\bin\SignedRelease\SanteDB.Messaging.HL7.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: ".\bin\SignedRelease\SanteDB.OrmLite.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: ".\bin\SignedRelease\SanteDB.Rest.AMI.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: ".\bin\SignedRelease\SanteDB.Rest.BIS.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: ".\bin\SignedRelease\SanteDB.Rest.Common.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: ".\bin\SignedRelease\SanteDB.Rest.HDSI.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: ".\bin\SignedRelease\SanteGuard.Core.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: ".\bin\SignedRelease\SanteGuard.Messaging.Syslog.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: ".\bin\SignedRelease\SharpCompress.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: ".\bin\SignedRelease\SqlCipher.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: ".\bin\SignedRelease\SQLite.Net.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: ".\bin\SignedRelease\SQLite.Net.Platform.*.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: ".\bin\SignedRelease\System.Net.IPNetwork.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: ".\bin\SignedRelease\sqlite3.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: ".\bin\SignedRelease\System.Buffers.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: ".\bin\SignedRelease\System.Data.Portable.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: ".\bin\SignedRelease\System.Diagnostics.PerformanceCounter.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: ".\bin\SignedRelease\System.Memory.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: ".\bin\SignedRelease\System.Numerics.Vectors.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: ".\bin\SignedRelease\System.Runtime.CompilerServices.Unsafe.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: ".\bin\SignedRelease\System.Security.AccessControl.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: ".\bin\SignedRelease\System.Security.Permissions.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: ".\bin\SignedRelease\System.Security.Principal.Windows.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: ".\bin\SignedRelease\System.Text.Encoding.CodePages.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: ".\bin\SignedRelease\System.Transactions.Portable.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: ".\bin\SignedRelease\fr\SanteDB.DisconnectedClient.i18n.resources.dll"; DestDir: "{app}\fr"; Flags: ignoreversion
Source: ".\bin\SignedRelease\Applets\*.pak"; DestDir: "{app}\Applets"; Flags: ignoreversion
Source: ".\bin\SignedRelease\ZXing*.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: ".\bin\SignedRelease\CoreCompat*.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: ".\bin\SignedRelease\santedb-dcg.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: ".\bin\SignedRelease\santedb-dcg.exe.config"; DestDir: "{app}"; Flags: ignoreversion
Source: ".\installsupp\restart.bat"; DestDir: "{app}"; Flags: ignoreversion;
Source: ".\installsupp\vcredist_x86.exe"; DestDir: "{tmp}"; Flags: dontcopy;
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
    ExtractTemporaryFile('vcredist_x86.exe');
    Exec(ExpandConstant('{tmp}\vcredist_x86.exe'), '/install /passive', '', SW_SHOW, ewWaitUntilTerminated, ResultCode);
    WizardForm.PreparingLabel.Caption := 'Installing Microsoft .NET Framework 4.8';
     ExtractTemporaryFile('netfx.exe');
    Exec(ExpandConstant('{tmp}\netfx.exe'), '/q', '', SW_SHOW, ewWaitUntilTerminated, ResultCode);
end;
