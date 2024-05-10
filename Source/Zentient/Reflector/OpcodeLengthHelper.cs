namespace Zentient.Reflector.Disassembler;

public static class OpcodeLengthHelper
{
    public const int OneByte = 1;
    public const int TwoBytes = 2;

    public static int DetermineBaseOpcodeLength(CilOpcode opcode)
    {
        switch (opcode)
        {
            // Opcodes with a base length of 1 byte
            case CilOpcode.Nop:
            case CilOpcode.Ldc_i4_s:
                // Add more opcodes as needed

                return OneByte;

            // Opcodes with a base length of 2 bytes
            case CilOpcode.Ldstr:
            case CilOpcode.Ldc_i4_0:
                // Add more opcodes as needed

                return TwoBytes;

            default:
                throw new InvalidOperationException($"Opcode {opcode} not recognized.");
        }
    }
}
