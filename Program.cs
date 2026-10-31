using System.IO.Compression;
using System.Reflection;
using System.Text;
using Zstandard.Net;
using TextCopy;

//The bytes that will represent the compressed output
byte[] decompressedBytes;

//args
string[] givenParameters;

//the actual data thing
string targetData;

/* flags */
bool flagClipboardGive = false;
bool flagWrite = false;

givenParameters = args;

//If the user shrimply launched the file
if (args.Length == 0)
{

    //Ask user for their given beta
    Console.WriteLine("Hello, World! Please insert your given ZStandard string below.");

    //Get Input
    givenParameters = Console.ReadLine().Split();
}

//Get argument given premeptively
targetData = givenParameters[0];

//For each karpument
foreach (var arg in givenParameters)
{
    
    switch (arg)
    {
        
        //Automatically output to a set file upon completion.
        case "-e":
            flagWrite = true;
            break;
        
        //Takes your clipboard as the input automatically instead
        case "-i":
            targetData = ClipboardService.GetText();
            break;
        
        //Adds the result to the clipboard automatically upon completion
        case "-o":
            flagClipboardGive = true;
            break;
        
        default:
            Console.WriteLine("Presuming " + arg + " is the given argument" );
            targetData = arg;
            break;
            
    }
    
}


Console.WriteLine("Processing.");

byte[] bytes;

//Try to convert to Base64
try
{
    bytes = Convert.FromBase64String(targetData);
}
catch (Exception e)
{
    Console.WriteLine("Failed to convert from Base64 because " + e.Message );
    
    throw;
}



//Attempt decompression
try
{
    using (var memoryStream = new MemoryStream(bytes))
    using (var compressionStream = new ZstandardStream(memoryStream, CompressionMode.Decompress))
    using (var outStream = new MemoryStream())
    {
        compressionStream.CopyTo(outStream);
        decompressedBytes = outStream.ToArray();
    }
    
}
//If it fails to for some reason.
catch (Exception e)
{
    Console.WriteLine("Failed to decompress Zstd from base64 object " + e + " " + e.Message);
    throw;
}

//Store our completion
string completion = Encoding.UTF8.GetString(decompressedBytes);

//Indicate our output
Console.WriteLine("Success.");
Console.WriteLine("output: " + completion );

//If we flag to save to clipboard
if (flagClipboardGive)
{
    try
    {
        //Do so
        ClipboardService.SetText(completion);
    }
    catch (Exception e)
    {
        Console.WriteLine("Failed to store to clipboard: " + e.Message);
        throw;
    }
    
}

if (flagWrite)
{
    string path = Path.Join(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "roblox_outputfile.txt");

    //Attempt to write file
    try
    {
        //Create the file
        FileStream file = File.Create(path);
        
        //Release its usage
        file.Close();
    }
    catch (Exception e)
    {
        Console.WriteLine("Could not create file because " + e.Message);
        throw;
    }
    
    //If the file was created at the path
    if (File.Exists(path))
    {
        try
        {
            //Write to it
            File.WriteAllText(path,completion);
        }
        catch (Exception e)
        {
            Console.WriteLine("Could not write to file because " + e.Message);
            throw;
        }
        
    }
    
}

