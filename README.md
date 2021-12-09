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
HTML5Packer.exe "C:\Users\Black7\source\repos\HTLML5Packer\.vs\HTML5Packer\HTML5Packer\bin\Debug\pack" css * 0 c:\packed.css
