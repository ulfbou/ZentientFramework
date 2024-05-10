namespace Zentient.Reflector.Disassembler;

public class OpcodeLengthCalculator
{
    public int CalculateTotalLength(CilOpcode opcode, byte[] bytecode)
    {
        // Determine base opcode length
        int baseLength = OpcodeLengthHelper.DetermineBaseOpcodeLength(opcode);

        // Parse operands
        OperandParser parser = new OperandParser();
        Operand[] operands = parser.ParseOperands(opcode, bytecode);

        // Calculate operand length
        int operandLength = CalculateOperandLength(operands);

        // Calculate total length
        int totalLength = baseLength + operandLength;

        return totalLength;
    }

    private int CalculateOperandLength(Operand[] operands)
    {
        int totalLength = 0;

        foreach (var operand in operands)
        {
            // Calculate length based on operand type
            if (operand is ImmediateOperand)
            {
                totalLength += sizeof(int); // Assuming immediate value is 4 bytes
            }
            else if (operand is MetadataTokenOperand)
            {
                totalLength += sizeof(int); // Assuming metadata token is 4 bytes
            }
            // Add cases for other operand types as needed
        }

        return totalLength;
    }

    public Dictionary<int, int> CalculateOpcodeLengths(byte[] bytecode)
    {
        Dictionary<int, int> opcodeLengths = new Dictionary<int, int>();
        int index = 0;

        while (index < bytecode.Length)
        {
            // Read opcode
            byte opcodeValue = bytecode[index];
            CilOpcode opcode = (CilOpcode)opcodeValue;

            // Determine total length
            int totalLength = CalculateTotalLength(opcode, bytecode.Skip(index).ToArray());

            // Store opcode length
            opcodeLengths[index] = totalLength;

            // Move to next instruction
            index += totalLength;
        }

        return opcodeLengths;
    }
}

