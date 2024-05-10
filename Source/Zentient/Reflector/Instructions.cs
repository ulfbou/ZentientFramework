using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zentient.Reflector.Disassembler
{
    public class Instructions
    {
        public struct Instruction
        {
            public string Name { get; set; }
            public string Desc { get; set; }
            public string Type { get; set; }
            public int Size { get; set; }

            public Instruction(string Name, string Desc, string Type, int Size = 1)
            {
                this.Name = Name ?? throw new ArgumentNullException(nameof(Name));
                this.Desc = Desc ?? throw new ArgumentNullException(nameof(Desc));
                this.Type = Type ?? throw new ArgumentNullException(nameof(Type));
                this.Size = Size; 
            }
        }

        private static ImmutableDictionary<int, Instruction> _instructions = null!;

        public static ImmutableDictionary<int, Instruction> Opcodes
        {
            get
            {
                if (_instructions == null)
                {
                    Init();
                }
                return _instructions!;
            }
            private set
            {
                _instructions = value;
            }
        }

        private static void Init()
        {
            Dictionary<int, Dictionary<string, string>> opcodes = new();
            try
            {
                #region init
                opcodes = new();
                opcodes[0x00] = new() { { "name", "nop" }, { "desc", "Do nothing (No operation)." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0x01] = new() { { "name", "break" }, { "desc", "Inform a debugger that a breakpoint has been reached." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0x02] = new() { { "name", "ldarg.0" }, { "desc", "Load argument 0 onto the stack." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0x03] = new() { { "name", "ldarg.1" }, { "desc", "Load argument 1 onto the stack." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0x04] = new() { { "name", "ldarg.2" }, { "desc", "Load argument 2 onto the stack." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0x05] = new() { { "name", "ldarg.3" }, { "desc", "Load argument 3 onto the stack." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0x06] = new() { { "name", "ldloc.0" }, { "desc", "Load local variable 0 onto stack." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0x07] = new() { { "name", "ldloc.1" }, { "desc", "Load local variable 1 onto stack." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0x08] = new() { { "name", "ldloc.2" }, { "desc", "Load local variable 2 onto stack." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0x09] = new() { { "name", "ldloc.3" }, { "desc", "Load local variable 3 onto stack." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0x0A] = new() { { "name", "stloc.0" }, { "desc", "Pop a value from stack into local variable 0." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0x0B] = new() { { "name", "stloc.1" }, { "desc", "Pop a value from stack into local variable 1." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0x0C] = new() { { "name", "stloc.2" }, { "desc", "Pop a value from stack into local variable 2." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0x0D] = new() { { "name", "stloc.3" }, { "desc", "Pop a value from stack into local variable 3." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0x0E] = new() { { "name", "ldarg.s <uint8 (num)>" }, { "desc", "Load argument numbered num onto the stack, short form." }, { "type", "Base instruction" }, { "size", "2" } };
                opcodes[0x0F] = new() { { "name", "ldarga.s <uint8 (argNum)>" }, { "desc", "Fetch the address of argument argNum, short form." }, { "type", "Base instruction" }, { "size", "2" } };
                opcodes[0x10] = new() { { "name", "starg.s <uint8 (num)>" }, { "desc", "Store value to the argument numbered num, short form." }, { "type", "Base instruction" }, { "size", "2" } };
                opcodes[0x11] = new() { { "name", "ldloc.s <uint8 (indx)>" }, { "desc", "Load local variable of index indx onto stack, short form." }, { "type", "Base instruction" }, { "size", "2" } };
                opcodes[0x12] = new() { { "name", "ldloca.s <uint8 (indx)>" }, { "desc", "Load address of local variable with index indx, short form." }, { "type", "Base instruction" }, { "size", "2" } };
                opcodes[0x13] = new() { { "name", "stloc.s <uint8 (indx)>" }, { "desc", "Pop a value from stack into local variable indx, short form." }, { "type", "Base instruction" }, { "size", "2" } };
                opcodes[0x14] = new() { { "name", "ldnull" }, { "desc", "Push a null reference on the stack." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0x15] = new() { { "name", "ldc.i4.M1" }, { "desc", "Push -1 onto the stack as int32 (alias for ldc.i4.m1)." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0x16] = new() { { "name", "ldc.i4.0" }, { "desc", "Push 0 onto the stack as int32." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0x17] = new() { { "name", "ldc.i4.1" }, { "desc", "Push 1 onto the stack as int32." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0x18] = new() { { "name", "ldc.i4.2" }, { "desc", "Push 2 onto the stack as int32." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0x19] = new() { { "name", "ldc.i4.3" }, { "desc", "Push 3 onto the stack as int32." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0x1A] = new() { { "name", "ldc.i4.4" }, { "desc", "Push 4 onto the stack as int32." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0x1B] = new() { { "name", "ldc.i4.5" }, { "desc", "Push 5 onto the stack as int32." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0x1C] = new() { { "name", "ldc.i4.6" }, { "desc", "Push 6 onto the stack as int32." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0x1D] = new() { { "name", "ldc.i4.7" }, { "desc", "Push 7 onto the stack as int32." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0x1E] = new() { { "name", "ldc.i4.8" }, { "desc", "Push 8 onto the stack as int32." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0x1F] = new() { { "name", "ldc.i4.s <int8 (num)>" }, { "desc", "Push num onto the stack as int32, short form." }, { "type", "Base instruction" }, { "size", "2" } };
                opcodes[0x20] = new() { { "name", "ldc.i4 <int32 (num)>" }, { "desc", "Push num of type int32 onto the stack as int32." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0x21] = new() { { "name", "ldc.i8 <int64 (num)>" }, { "desc", "Push num of type int64 onto the stack as int64." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0x22] = new() { { "name", "ldc.r4 <float32 (num)>" }, { "desc", "Push num of type float32 onto the stack as F." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0x23] = new() { { "name", "ldc.r8 <float64 (num)>" }, { "desc", "Push num of type float64 onto the stack as F." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0x25] = new() { { "name", "dup" }, { "desc", "Duplicate the value on the top of the stack." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0x26] = new() { { "name", "pop" }, { "desc", "Pop value from the stack." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0x27] = new() { { "name", "jmp <method>" }, { "desc", "Exit current method and jump to the specified method." }, { "type", "Base instruction" }, { "size", "*" } };
                opcodes[0x28] = new() { { "name", "call <method>" }, { "desc", "Call method described by method." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0x29] = new() { { "name", "calli <callsitedescr>" }, { "desc", "Call method indicated on the stack with arguments described by callsitedescr." }, { "type", "Base instruction" }, { "size", "*" } };
                opcodes[0x2A] = new() { { "name", "ret" }, { "desc", "Return from method, possibly with a value." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0x2B] = new() { { "name", "br.s <int8 (target)>" }, { "desc", "Branch to target, short form." }, { "type", "Base instruction" }, { "size", "2" } };
                opcodes[0x2C] = new() { { "name", "brfalse.s <int8 (target)>" }, { "desc", "Branch to target if value is zero (false), short form." }, { "type", "Base instruction" }, { "size", "2" } };
                opcodes[0x2D] = new() { { "name", "brinst.s <int8 (target)>" }, { "desc", "Branch to target if value is a non-null object reference, short form (alias for brtrue.s)." }, { "type", "Base instruction" }, { "size", "2" } };
                opcodes[0x2E] = new() { { "name", "beq.s <int8 (target)>" }, { "desc", "Branch to target if equal, short form." }, { "type", "Base instruction" }, { "size", "2" } };
                opcodes[0x2F] = new() { { "name", "bge.s <int8 (target)>" }, { "desc", "Branch to target if greater than or equal to, short form." }, { "type", "Base instruction" }, { "size", "2" } };
                opcodes[0x30] = new() { { "name", "bgt.s <int8 (target)>" }, { "desc", "Branch to target if greater than, short form." }, { "type", "Base instruction" }, { "size", "2" } };
                opcodes[0x31] = new() { { "name", "ble.s <int8 (target)>" }, { "desc", "Branch to target if less than or equal to, short form." }, { "type", "Base instruction" }, { "size", "2" } };
                opcodes[0x32] = new() { { "name", "blt.s <int8 (target)>" }, { "desc", "Branch to target if less than, short form." }, { "type", "Base instruction" }, { "size", "2" } };
                opcodes[0x33] = new() { { "name", "bne.un.s <int8 (target)>" }, { "desc", "Branch to target if unequal or unordered, short form." }, { "type", "Base instruction" }, { "size", "2" } };
                opcodes[0x34] = new() { { "name", "bge.un.s <int8 (target)>" }, { "desc", "Branch to target if greater than or equal to (unsigned or unordered), short form." }, { "type", "Base instruction" }, { "size", "2" } };
                opcodes[0x35] = new() { { "name", "bgt.un.s <int8 (target)>" }, { "desc", "Branch to target if greater than (unsigned or unordered), short form." }, { "type", "Base instruction" }, { "size", "2" } };
                opcodes[0x36] = new() { { "name", "ble.un.s <int8 (target)>" }, { "desc", "Branch to target if less than or equal to (unsigned or unordered), short form." }, { "type", "Base instruction" }, { "size", "2" } };
                opcodes[0x37] = new() { { "name", "blt.un.s <int8 (target)>" }, { "desc", "Branch to target if less than (unsigned or unordered), short form." }, { "type", "Base instruction" }, { "size", "2" } };
                opcodes[0x38] = new() { { "name", "br <int32 (target)>" }, { "desc", "Branch to target." }, { "type", "Base instruction" }, { "size", "5" } };
                opcodes[0x39] = new() { { "name", "brfalse <int32 (target)>" }, { "desc", "Branch to target if value is zero (false)." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0x3A] = new() { { "name", "brinst <int32 (target)>" }, { "desc", "Branch to target if value is a non-null object reference (alias for brtrue)." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0x3B] = new() { { "name", "beq <int32 (target)>" }, { "desc", "Branch to target if equal." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0x3C] = new() { { "name", "bge <int32 (target)>" }, { "desc", "Branch to target if greater than or equal to." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0x3D] = new() { { "name", "bgt <int32 (target)>" }, { "desc", "Branch to target if greater than." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0x3E] = new() { { "name", "ble <int32 (target)>" }, { "desc", "Branch to target if less than or equal to." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0x3F] = new() { { "name", "blt <int32 (target)>" }, { "desc", "Branch to target if less than." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0x40] = new() { { "name", "bne.un <int32 (target)>" }, { "desc", "Branch to target if unequal or unordered." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0x41] = new() { { "name", "bge.un <int32 (target)>" }, { "desc", "Branch to target if greater than or equal to (unsigned or unordered)." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0x42] = new() { { "name", "bgt.un <int32 (target)>" }, { "desc", "Branch to target if greater than (unsigned or unordered)." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0x43] = new() { { "name", "ble.un <int32 (target)>" }, { "desc", "Branch to target if less than or equal to (unsigned or unordered)." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0x44] = new() { { "name", "blt.un <int32 (target)>" }, { "desc", "Branch to target if less than (unsigned or unordered)." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0x45] = new() { { "name", "switch <uint32, int32, int32 (t1..tN)>" }, { "desc", "Jump to one of n values." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0x46] = new() { { "name", "ldind.i1" }, { "desc", "Indirect load value of type int8 as int32 on the stack." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0x47] = new() { { "name", "ldind.u1" }, { "desc", "Indirect load value of type unsigned int8 as int32 on the stack." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0x48] = new() { { "name", "ldind.i2" }, { "desc", "Indirect load value of type int16 as int32 on the stack." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0x49] = new() { { "name", "ldind.u2" }, { "desc", "Indirect load value of type unsigned int16 as int32 on the stack." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0x4A] = new() { { "name", "ldind.i4" }, { "desc", "Indirect load value of type int32 as int32 on the stack." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0x4B] = new() { { "name", "ldind.u4" }, { "desc", "Indirect load value of type unsigned int32 as int32 on the stack." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0x4C] = new() { { "name", "ldind.i8" }, { "desc", "Indirect load value of type int64 as int64 on the stack." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0x4C] = new() { { "name", "ldind.u8" }, { "desc", "Indirect load value of type unsigned int64 as int64 on the stack (alias for ldind.i8)." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0x4D] = new() { { "name", "ldind.i" }, { "desc", "Indirect load value of type native int as native int on the stack." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0x4E] = new() { { "name", "ldind.r4" }, { "desc", "Indirect load value of type float32 as F on the stack." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0x4F] = new() { { "name", "ldind.r8" }, { "desc", "Indirect load value of type float64 as F on the stack." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0x50] = new() { { "name", "ldind.ref" }, { "desc", "Indirect load value of type object ref as O on the stack." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0x51] = new() { { "name", "stind.ref" }, { "desc", "Store value of type object ref (type O) into memory at address." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0x52] = new() { { "name", "stind.i1" }, { "desc", "Store value of type int8 into memory at address." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0x53] = new() { { "name", "stind.i2" }, { "desc", "Store value of type int16 into memory at address." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0x54] = new() { { "name", "stind.i4" }, { "desc", "Store value of type int32 into memory at address." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0x55] = new() { { "name", "stind.i8" }, { "desc", "Store value of type int64 into memory at address." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0x56] = new() { { "name", "stind.r4" }, { "desc", "Store value of type float32 into memory at address." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0x57] = new() { { "name", "stind.r8" }, { "desc", "Store value of type float64 into memory at address." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0x58] = new() { { "name", "add" }, { "desc", "Add two values, returning a new value." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0x59] = new() { { "name", "sub" }, { "desc", "Subtract value2 from value1, returning a new value." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0x5A] = new() { { "name", "mul" }, { "desc", "Multiply values." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0x5B] = new() { { "name", "div" }, { "desc", "Divide two values to return a quotient or floating-point result." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0x5C] = new() { { "name", "div.un" }, { "desc", "Divide two values, unsigned, returning a quotient." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0x5D] = new() { { "name", "rem" }, { "desc", "Remainder when dividing one value by another." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0x5E] = new() { { "name", "rem.un" }, { "desc", "Remainder when dividing one unsigned value by another." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0x5F] = new() { { "name", "and" }, { "desc", "Bitwise AND of two integral values, returns an integral value." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0x60] = new() { { "name", "or" }, { "desc", "Bitwise OR of two integer values, returns an integer." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0x61] = new() { { "name", "xor" }, { "desc", "Bitwise XOR of integer values, returns an integer." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0x62] = new() { { "name", "shl" }, { "desc", "Shift an integer left (shifting in zeros), return an integer." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0x63] = new() { { "name", "shr" }, { "desc", "Shift an integer right (shift in sign), return an integer." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0x64] = new() { { "name", "shr.un" }, { "desc", "Shift an integer right (shift in zero), return an integer." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0x65] = new() { { "name", "neg" }, { "desc", "Negate value." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0x66] = new() { { "name", "not" }, { "desc", "Bitwise complement." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0x67] = new() { { "name", "conv.i1" }, { "desc", "Convert to int8, pushing int32 on stack." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0x68] = new() { { "name", "conv.i2" }, { "desc", "Convert to int16, pushing int32 on stack." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0x69] = new() { { "name", "conv.i4" }, { "desc", "Convert to int32, pushing int32 on stack." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0x6A] = new() { { "name", "conv.i8" }, { "desc", "Convert to int64, pushing int64 on stack." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0x6B] = new() { { "name", "conv.r4" }, { "desc", "Convert to float32, pushing F on stack." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0x6C] = new() { { "name", "conv.r8" }, { "desc", "Convert to float64, pushing F on stack." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0x6D] = new() { { "name", "conv.u4" }, { "desc", "Convert to unsigned int32, pushing int32 on stack." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0x6E] = new() { { "name", "conv.u8" }, { "desc", "Convert to unsigned int64, pushing int64 on stack." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0x6F] = new() { { "name", "callvirt <method>" }, { "desc", "Call a method associated with an object." }, { "type", "Object model instruction" }, { "size", "1" } };
                opcodes[0x70] = new() { { "name", "cpobj <typeTok>" }, { "desc", "Copy a value type from src to dest." }, { "type", "Object model instruction" }, { "size", "1" } };
                opcodes[0x71] = new() { { "name", "ldobj <typeTok>" }, { "desc", "Copy the value stored at address src to the stack." }, { "type", "Object model instruction" }, { "size", "1" } };
                opcodes[0x72] = new() { { "name", "ldstr <string>" }, { "desc", "Push a string object for the literal string." }, { "type", "Object model instruction" }, { "size", "1" } };
                opcodes[0x73] = new() { { "name", "newobj <ctor>" }, { "desc", "Allocate an uninitialized object or value type and call ctor." }, { "type", "Object model instruction" }, { "size", "1" } };
                opcodes[0x74] = new() { { "name", "castclass <class>" }, { "desc", "Cast obj to class." }, { "type", "Object model instruction" }, { "size", "1" } };
                opcodes[0x75] = new() { { "name", "isinst <class>" }, { "desc", "Test if obj is an instance of class, returning null or an instance of that class or interface." }, { "type", "Object model instruction" }, { "size", "1" } };
                opcodes[0x76] = new() { { "name", "conv.r.un" }, { "desc", "Convert unsigned integer to floating-point, pushing F on stack." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0x79] = new() { { "name", "unbox <valuetype>" }, { "desc", "Extract a value-type from obj, its boxed representation, and push a controlled-mutability managed pointer to it to the top of the stack." }, { "type", "Object model instruction" }, { "size", "1" } };
                opcodes[0x7A] = new() { { "name", "throw" }, { "desc", "Throw an exception." }, { "type", "Object model instruction" }, { "size", "1" } };
                opcodes[0x7B] = new() { { "name", "ldfld <field>" }, { "desc", "Push the value of field of object (or value type) obj, onto the stack." }, { "type", "Object model instruction" }, { "size", "1" } };
                opcodes[0x7C] = new() { { "name", "ldflda <field>" }, { "desc", "Push the address of field of object obj on the stack." }, { "type", "Object model instruction" }, { "size", "1" } };
                opcodes[0x7D] = new() { { "name", "stfld <field>" }, { "desc", "Replace the value of field of the object obj with value." }, { "type", "Object model instruction" }, { "size", "1" } };
                opcodes[0x7E] = new() { { "name", "ldsfld <field>" }, { "desc", "Push the value of the static field on the stack." }, { "type", "Object model instruction" }, { "size", "1" } };
                opcodes[0x7F] = new() { { "name", "ldsflda <field>" }, { "desc", "Push the address of the static field, field, on the stack." }, { "type", "Object model instruction" }, { "size", "1" } };
                opcodes[0x80] = new() { { "name", "stsfld <field>" }, { "desc", "Replace the value of the static field with val." }, { "type", "Object model instruction" }, { "size", "1" } };
                opcodes[0x81] = new() { { "name", "stobj <typeTok>" }, { "desc", "Store a value of type typeTok at an address." }, { "type", "Object model instruction" }, { "size", "1" } };
                opcodes[0x82] = new() { { "name", "conv.ovf.i1.un" }, { "desc", "Convert unsigned to an int8 (on the stack as int32) and throw an exception on overflow." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0x83] = new() { { "name", "conv.ovf.i2.un" }, { "desc", "Convert unsigned to an int16 (on the stack as int32) and throw an exception on overflow." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0x84] = new() { { "name", "conv.ovf.i4.un" }, { "desc", "Convert unsigned to an int32 (on the stack as int32) and throw an exception on overflow." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0x85] = new() { { "name", "conv.ovf.i8.un" }, { "desc", "Convert unsigned to an int64 (on the stack as int64) and throw an exception on overflow." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0x86] = new() { { "name", "conv.ovf.u1.un" }, { "desc", "Convert unsigned to an unsigned int8 (on the stack as int32) and throw an exception on overflow." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0x87] = new() { { "name", "conv.ovf.u2.un" }, { "desc", "Convert unsigned to an unsigned int16 (on the stack as int32) and throw an exception on overflow." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0x88] = new() { { "name", "conv.ovf.u4.un" }, { "desc", "Convert unsigned to an unsigned int32 (on the stack as int32) and throw an exception on overflow." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0x89] = new() { { "name", "conv.ovf.u8.un" }, { "desc", "Convert unsigned to an unsigned int64 (on the stack as int64) and throw an exception on overflow." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0x8A] = new() { { "name", "conv.ovf.i.un" }, { "desc", "Convert unsigned to a native int (on the stack as native int) and throw an exception on overflow." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0x8B] = new() { { "name", "conv.ovf.u.un" }, { "desc", "Convert unsigned to a native unsigned int (on the stack as native int) and throw an exception on overflow." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0x8C] = new() { { "name", "box <typeTok>" }, { "desc", "Convert a boxable value to its boxed form." }, { "type", "Object model instruction" }, { "size", "1" } };
                opcodes[0x8D] = new() { { "name", "newarr <etype>" }, { "desc", "Create a new array with elements of type etype." }, { "type", "Object model instruction" }, { "size", "1" } };
                opcodes[0x8E] = new() { { "name", "ldlen" }, { "desc", "Push the length (of type native unsigned int) of array on the stack." }, { "type", "Object model instruction" }, { "size", "1" } };
                opcodes[0x8F] = new() { { "name", "ldelema <class>" }, { "desc", "Load the address of element at index onto the top of the stack." }, { "type", "Object model instruction" }, { "size", "1" } };
                opcodes[0x90] = new() { { "name", "ldelem.i1" }, { "desc", "Load the element with type int8 at index onto the top of the stack as an int32." }, { "type", "Object model instruction" }, { "size", "1" } };
                opcodes[0x91] = new() { { "name", "ldelem.u1" }, { "desc", "Load the element with type unsigned int8 at index onto the top of the stack as an int32." }, { "type", "Object model instruction" }, { "size", "1" } };
                opcodes[0x92] = new() { { "name", "ldelem.i2" }, { "desc", "Load the element with type int16 at index onto the top of the stack as an int32." }, { "type", "Object model instruction" }, { "size", "1" } };
                opcodes[0x93] = new() { { "name", "ldelem.u2" }, { "desc", "Load the element with type unsigned int16 at index onto the top of the stack as an int32." }, { "type", "Object model instruction" }, { "size", "1" } };
                opcodes[0x94] = new() { { "name", "ldelem.i4" }, { "desc", "Load the element with type int32 at index onto the top of the stack as an int32." }, { "type", "Object model instruction" }, { "size", "1" } };
                opcodes[0x95] = new() { { "name", "ldelem.u4" }, { "desc", "Load the element with type unsigned int32 at index onto the top of the stack as an int32." }, { "type", "Object model instruction" }, { "size", "1" } };
                opcodes[0x96] = new() { { "name", "ldelem.i8" }, { "desc", "Load the element with type int64 at index onto the top of the stack as an int64." }, { "type", "Object model instruction" }, { "size", "1" } };
                opcodes[0x96] = new() { { "name", "ldelem.u8" }, { "desc", "Load the element with type unsigned int64 at index onto the top of the stack as an int64 (alias for ldelem.i8)." }, { "type", "Object model instruction" }, { "size", "1" } };
                opcodes[0x97] = new() { { "name", "ldelem.i" }, { "desc", "Load the element with type native int at index onto the top of the stack as a native int." }, { "type", "Object model instruction" }, { "size", "1" } };
                opcodes[0x98] = new() { { "name", "ldelem.r4" }, { "desc", "Load the element with type float32 at index onto the top of the stack as an F." }, { "type", "Object model instruction" }, { "size", "1" } };
                opcodes[0x99] = new() { { "name", "ldelem.r8" }, { "desc", "Load the element with type float64 at index onto the top of the stack as an F." }, { "type", "Object model instruction" }, { "size", "1" } };
                opcodes[0x9A] = new() { { "name", "ldelem.ref" }, { "desc", "Load the element at index onto the top of the stack as an O. The type of the O is the same as the element type of the array pushed on the CIL stack." }, { "type", "Object model instruction" }, { "size", "1" } };
                opcodes[0x9B] = new() { { "name", "stelem.i" }, { "desc", "Replace array element at index with the native int value on the stack." }, { "type", "Object model instruction" }, { "size", "1" } };
                opcodes[0x9C] = new() { { "name", "stelem.i1" }, { "desc", "Replace array element at index with the int8 value on the stack." }, { "type", "Object model instruction" }, { "size", "1" } };
                opcodes[0x9D] = new() { { "name", "stelem.i2" }, { "desc", "Replace array element at index with the int16 value on the stack." }, { "type", "Object model instruction" }, { "size", "1" } };
                opcodes[0x9E] = new() { { "name", "stelem.i4" }, { "desc", "Replace array element at index with the int32 value on the stack." }, { "type", "Object model instruction" }, { "size", "1" } };
                opcodes[0x9F] = new() { { "name", "stelem.i8" }, { "desc", "Replace array element at index with the int64 value on the stack." }, { "type", "Object model instruction" }, { "size", "1" } };
                opcodes[0xA0] = new() { { "name", "stelem.r4" }, { "desc", "Replace array element at index with the float32 value on the stack." }, { "type", "Object model instruction" }, { "size", "1" } };
                opcodes[0xA1] = new() { { "name", "stelem.r8" }, { "desc", "Replace array element at index with the float64 value on the stack." }, { "type", "Object model instruction" }, { "size", "1" } };
                opcodes[0xA2] = new() { { "name", "stelem.ref" }, { "desc", "Replace array element at index with the ref value on the stack." }, { "type", "Object model instruction" }, { "size", "1" } };
                opcodes[0xA3] = new() { { "name", "ldelem <typeTok>" }, { "desc", "Load the element at index onto the top of the stack." }, { "type", "Object model instruction" }, { "size", "1" } };
                opcodes[0xA4] = new() { { "name", "stelem <typeTok>" }, { "desc", "Replace array element at index with the value on the stack." }, { "type", "Object model instruction" }, { "size", "1" } };
                opcodes[0xA5] = new() { { "name", "unbox.any <typeTok>" }, { "desc", "Extract a value-type from obj, its boxed representation, and copy to the top of the stack." }, { "type", "Object model instruction" }, { "size", "1" } };
                opcodes[0xB3] = new() { { "name", "conv.ovf.i1" }, { "desc", "Convert to an int8 (on the stack as int32) and throw an exception on overflow." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0xB4] = new() { { "name", "conv.ovf.u1" }, { "desc", "Convert to an unsigned int8 (on the stack as int32) and throw an exception on overflow." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0xB5] = new() { { "name", "conv.ovf.i2" }, { "desc", "Convert to an int16 (on the stack as int32) and throw an exception on overflow." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0xB6] = new() { { "name", "conv.ovf.u2" }, { "desc", "Convert to an unsigned int16 (on the stack as int32) and throw an exception on overflow." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0xB7] = new() { { "name", "conv.ovf.i4" }, { "desc", "Convert to an int32 (on the stack as int32) and throw an exception on overflow." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0xB8] = new() { { "name", "conv.ovf.u4" }, { "desc", "Convert to an unsigned int32 (on the stack as int32) and throw an exception on overflow." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0xB9] = new() { { "name", "conv.ovf.i8" }, { "desc", "Convert to an int64 (on the stack as int64) and throw an exception on overflow." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0xBA] = new() { { "name", "conv.ovf.u8" }, { "desc", "Convert to an unsigned int64 (on the stack as int64) and throw an exception on overflow." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0xC2] = new() { { "name", "refanyval <type>" }, { "desc", "Push the address stored in a typed reference." }, { "type", "Object model instruction" }, { "size", "1" } };
                opcodes[0xC3] = new() { { "name", "ckfinite" }, { "desc", "Throw ArithmeticException if value is not a finite number." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0xC6] = new() { { "name", "mkrefany <class>" }, { "desc", "Push a typed reference to ptr of type class onto the stack." }, { "type", "Object model instruction" }, { "size", "1" } };
                opcodes[0xD0] = new() { { "name", "ldtoken <token>" }, { "desc", "Convert metadata token to its runtime representation." }, { "type", "Object model instruction" }, { "bytesize", "1" }, { "size", "1" } };
                opcodes[0xD1] = new() { { "name", "conv.u2" }, { "desc", "Convert to unsigned int16, pushing int32 on stack." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0xD2] = new() { { "name", "conv.u1" }, { "desc", "Convert to unsigned int8, pushing int32 on stack." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0xD3] = new() { { "name", "conv.i" }, { "desc", "Convert to native int, pushing native int on stack." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0xD4] = new() { { "name", "conv.ovf.i" }, { "desc", "Convert to a native int (on the stack as native int) and throw an exception on overflow." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0xD5] = new() { { "name", "conv.ovf.u" }, { "desc", "Convert to a native unsigned int (on the stack as native int) and throw an exception on overflow." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0xD6] = new() { { "name", "add.ovf" }, { "desc", "Add signed integer values with overflow check." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0xD7] = new() { { "name", "add.ovf.un" }, { "desc", "Add unsigned integer values with overflow check." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0xD8] = new() { { "name", "mul.ovf" }, { "desc", "Multiply signed integer values. Signed result shall fit in same size." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0xD9] = new() { { "name", "mul.ovf.un" }, { "desc", "Multiply unsigned integer values. Unsigned result shall fit in same size." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0xDA] = new() { { "name", "sub.ovf" }, { "desc", "Subtract native int from a native int. Signed result shall fit in same size." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0xDB] = new() { { "name", "sub.ovf.un" }, { "desc", "Subtract native unsigned int from a native unsigned int. Unsigned result shall fit in same size." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0xDC] = new() { { "name", "endfault" }, { "desc", "End fault clause of an exception block." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0xDC] = new() { { "name", "endfinally" }, { "desc", "End finally clause of an exception block." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0xDD] = new() { { "name", "leave <int32 (target)>" }, { "desc", "Exit a protected region of code." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0xDE] = new() { { "name", "leave.s <int8 (target)>" }, { "desc", "Exit a protected region of code, short form." }, { "type", "Base instruction" }, { "size", "2" } };
                opcodes[0xDF] = new() { { "name", "stind.i" }, { "desc", "Store value of type native int into memory at address." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0xE0] = new() { { "name", "conv.u" }, { "desc", "Convert to native unsigned int, pushing native int on stack." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0xFE00] = new() { { "name", "arglist" }, { "desc", "Return argument list handle for the current method." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0xFE01] = new() { { "name", "ceq" }, { "desc", "Push 1 (of type int32) if value1 equals value2, else push 0." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0xFE02] = new() { { "name", "cgt" }, { "desc", "Push 1 (of type int32) if value1 greater that value2, else push 0." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0xFE03] = new() { { "name", "cgt.un" }, { "desc", "Push 1 (of type int32) if value1 greater that value2, unsigned or unordered, else push 0." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0xFE04] = new() { { "name", "clt" }, { "desc", "Push 1 (of type int32) if value1 lower than value2, else push 0." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0xFE05] = new() { { "name", "clt.un" }, { "desc", "Push 1 (of type int32) if value1 lower than value2, unsigned or unordered, else push 0." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0xFE06] = new() { { "name", "ldftn <method>" }, { "desc", "Push a pointer to a method referenced by method, on the stack." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0xFE07] = new() { { "name", "ldvirtftn <method>" }, { "desc", "Push address of virtual method on the stack." }, { "type", "Object model instruction" }, { "size", "1" } };
                opcodes[0xFE09] = new() { { "name", "ldarg <uint16 (num)>" }, { "desc", "Load argument numbered num onto the stack." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0xFE0A] = new() { { "name", "ldarga <uint16 (argNum)>" }, { "desc", "Fetch the address of argument argNum." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0xFE0B] = new() { { "name", "starg <uint16 (num)>" }, { "desc", "Store value to the argument numbered num." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0xFE0C] = new() { { "name", "ldloc <uint16 (indx)>" }, { "desc", "Load local variable of index indx onto stack." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0xFE0D] = new() { { "name", "ldloca <uint16 (indx)>" }, { "desc", "Load address of local variable with index indx." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0xFE0E] = new() { { "name", "stloc <uint16 (indx)>" }, { "desc", "Pop a value from stack into local variable indx." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0xFE0F] = new() { { "name", "localloc" }, { "desc", "Allocate space from the local memory pool." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0xFE11] = new() { { "name", "endfilter" }, { "desc", "End an exception handling filter clause." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0xFE12] = new() { { "name", "unaligned. (alignment)" }, { "desc", "Subsequent pointer instruction might be unaligned." }, { "type", "Prefix to instruction" }, { "size", "1" } };
                opcodes[0xFE13] = new() { { "name", "volatile." }, { "desc", "Subsequent pointer reference is volatile." }, { "type", "Prefix to instruction" }, { "size", "1" } };
                opcodes[0xFE14] = new() { { "name", "tail." }, { "desc", "Subsequent call terminates current method." }, { "type", "Prefix to instruction" }, { "size", "1" } };
                opcodes[0xFE15] = new() { { "name", "initobj <typeTok>" }, { "desc", "Initialize the value at address dest." }, { "type", "Object model instruction" }, { "size", "1" } };
                opcodes[0xFE16] = new() { { "name", "constrained. <thisType>" }, { "desc", "Call a virtual method on a type constrained to be type T." }, { "type", "Prefix to instruction" }, { "size", "1" } };
                opcodes[0xFE17] = new() { { "name", "cpblk" }, { "desc", "Copy data from memory to memory." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0xFE18] = new() { { "name", "initblk" }, { "desc", "Set all bytes in a block of memory to a given byte value." }, { "type", "Base instruction" }, { "size", "1" } };
                opcodes[0xFE19] = new() { { "name", "no. {typecheck, rangecheck, nullcheck }" }, { "desc", "The specified fault check(s) normally performed as part of the execution of the subsequent instruction can/shall be skipped." }, { "type", "Prefix to instruction" }, { "size", "1" } };
                opcodes[0xFE1A] = new() { { "name", "rethrow" }, { "desc", "Rethrow the current exception." }, { "type", "Object model instruction" }, { "size", "2" } };
                opcodes[0xFE1C] = new() { { "name", "sizeof <typeTok>" }, { "desc", "Push the size, in bytes, of a type as an unsigned int32." }, { "type", "Object model instruction" }, { "size", "2" } };
                opcodes[0xFE1D] = new() { { "name", "refanytype" }, { "desc", "Push the type token stored in a typed reference." }, { "type", "Object model instruction" }, { "size", "1" } };
                opcodes[0xFE1E] = new() { { "name", "readonly." }, { "desc", "Specify that the subsequent array address operation performs no type check at runtime, and that it returns a controlled-mutability managed pointer." }, { "type", "Prefix to instruction" }, { "size", "1" } };
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR1d: {ex.Message}");
            }

            Dictionary<int, Instruction> instructions = new();

            try
            {
                instructions = new Dictionary<int, Instruction>();

                foreach (var kvp in opcodes)
                {
                    var instruction = kvp.Value;
                    var bytesize = instruction.ContainsKey("size") ? instruction["size"] : "1";
                    int size;
                    try
                    {
                        size = int.Parse(bytesize);
                    }
                    catch
                    {
                        size = 1;
                    }
                    instructions[kvp.Key] = new Instruction
                    {
                        Name = instruction["name"],
                        Desc = instruction["desc"],
                        Type = instruction["type"],
                        Size = size
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR2: {ex.Message}");
            }

            try
            {
                _instructions = instructions.ToImmutableDictionary<int, Instruction>();

            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR3: {ex.Message}");
            }
        }
    }
}
