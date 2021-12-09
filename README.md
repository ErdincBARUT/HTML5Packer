# HTML5Packer
Packing resources (image,sound..) into JS or CSS by Base64 encode.

Commandline tool for packing Resources into Javascript , Css
That tool useful for building html5 apps. It can be solve load problems in your html5 app.

Arguments:
<TargetPath>       = Target resource directory (including png,gif,jpg,ogg...)
<exportType>       = js or css (export format)
<PackSize>         = Maximum export file length (bytes),app can split resources into multiple files. 0 means all in one file.
<TargetOutputFile> = Path of exported file css or js

Parameters:
--topfolder   =  Include resources only in top directory (not including subdirectories)
--nooutput    = No any output into console

Example:
HTML5Packer.exe "<TargetPath>" <exportType> <FormatTypes> <PackSize> <TargetOutputFile> --topfolder --nooutput
Pack to JS:
HTML5Packer.exe "C:\pack" js * 0 c:\packed.js
  
Pack to Css
HTML5Packer.exe "C:\pack" css * 0 c:\packed.css

 SplitFiles by 512kb:
  HTML5Packer.exe "C:\pack" js * 512000 c:\packed.js
  
 Include only png,gif :
  HTML5Packer.exe "C:\pack" js png,gif 0 c:\packed.js
