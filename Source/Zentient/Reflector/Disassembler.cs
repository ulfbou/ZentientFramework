using System;
using System.IO;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;

public class Disassembler
{
    public static void Disassemble(string filePath)
    {
        using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
        {
            using (PEReader peReader = new PEReader(fs))
            {
                MetadataReader metadataReader = peReader.GetMetadataReader();

                foreach (var methodHandle in metadataReader.MethodDefinitions)
                {
                    MethodDefinition method = metadataReader.GetMethodDefinition(methodHandle);
                    string methodName = metadataReader.GetString(method.Name);
                    Console.WriteLine($"Method: {methodName}");

                    if (method.RelativeVirtualAddress != 0)
                    {
                        // Get the method body
                        MethodBodyBlock methodBody = peReader.GetMethodBody(method.RelativeVirtualAddress);
                        BlobReader ilReader = methodBody.GetILReader();

                        while (ilReader.RemainingBytes > 0)
                        {
                            // Read the IL instruction and its operands here
                            // This is a simplified example, actual IL reading is more complex
                            byte opCode = ilReader.ReadByte();
                            Console.WriteLine($"IL OpCode: {opCode}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Method has no IL body.");
                    }
                }

            }
        }
    }
}
