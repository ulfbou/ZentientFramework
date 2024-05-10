namespace Zentient.Reflector.Disassembler;

public class Operand
{
    // Base class for operands
}

// Define other operand types as needed
public class ImmediateOperand : Operand
{
    // Represents immediate values
    public int Value { get; set; }
}

public class MetadataTokenOperand : Operand
{
    // Represents metadata tokens
    public int Token { get; set; }
}

// Define other operand types as needed
