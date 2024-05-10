namespace Zentient.Reflector.Disassembler;

public class OperandParser
{
    public Operand[] ParseOperands(CilOpcode opcode, byte[] bytecode)
    {
        switch (opcode)
        {
            case CilOpcode.Ldc_i4:
                // Parse immediate value operand
                int value = BitConverter.ToInt32(bytecode, 1); // Assuming immediate value is 4 bytes
                return new Operand[] { new ImmediateOperand { Value = value } };

            case CilOpcode.Ldstr:
                // Parse metadata token operand
                int token = BitConverter.ToInt32(bytecode, 1); // Assuming metadata token is 4 bytes
                return new Operand[] { new MetadataTokenOperand { Token = token } };

            // Add cases for other opcodes as needed

            default:
                return new Operand[0]; // No operands
        }
    }
}
