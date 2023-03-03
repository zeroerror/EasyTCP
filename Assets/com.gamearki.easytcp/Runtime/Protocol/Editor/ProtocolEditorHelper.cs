using System;
using System.IO;
using System.Text;

namespace ZeroFrame.Protocol.Editor
{

    public static class ProtocolEditorHelper
    {

        public static void ProtocolGenAll(string dir, string servicePath, int maxMsgSize = 1000)
        {
            ClassEditor classEditor = new ClassEditor(File.ReadAllText(servicePath));
            classEditor.RemoveAllField();
            classEditor.AddUsing("System");
            classEditor.AddUsing("System.Collections.Generic");
            classEditor.AddField("Dictionary<Type, ushort> messageInfoDic;");

            StringBuilder sb = new StringBuilder();
            sb.Append(@"  

        public ProtocolService()
        {
            this.messageInfoDic = new Dictionary<Type, ushort>();
"
            );

            DirectoryInfo dirInfo = new DirectoryInfo(dir);
            int msgIndex = 0;
            foreach (FileInfo info in dirInfo.GetFiles("*.cs", SearchOption.AllDirectories))
            {
                string path = info.FullName;
                ProtocolGen(path, maxMsgSize, ref sb, ref msgIndex);

                ClassEditor _classEditor = new ClassEditor(File.ReadAllText(path));
            }
            Console.WriteLine("协议全部生成完毕！");
            sb.AppendLine(@"
        }");

            MethodEditor methodEditor1 = new MethodEditor(sb.ToString());
            MethodEditor methodEditor2 = new MethodEditor(@"

        public (byte serviceID, byte messageID) GetMessageID<T>()
        where T : IZeroMessage<T>
        {
            var type = typeof(T);
            bool hasMessage = messageInfoDic.TryGetValue(type, out ushort value);
            if (hasMessage)
            {
                return ((byte)value, (byte)(value >> 8));
            }
            else
            {
                throw new Exception();
            }
        }");

            classEditor.RemoveMethod("ProtocolService");
            classEditor.RemoveMethod("GetMessageID");
            classEditor.AddMethod(methodEditor1);
            classEditor.AddMethod(methodEditor2);
            File.WriteAllText(servicePath, classEditor.Text);

            Console.WriteLine("协议服务全部生成完毕！");
        }

        public static void ProtocolGen(string path, int msgMaxSize, ref StringBuilder sb, ref int msgIndex)
        {
            string originCode = File.ReadAllText(path);
            ClassEditor classEditor = new ClassEditor(originCode);
            if (!classEditor.IsClassHasAttribute("ZeroMessage")) return;

            sb.AppendLine($"            messageInfoDic.Add(typeof({classEditor.GetClassName()}), {msgIndex++});");
            // Add Using
            classEditor.AddUsing("ZeroFrame.Buffer");

            // Add Interface
            var name = classEditor.GetClassName();
            classEditor.RemoveInterface("IZeroMessage", name);
            classEditor.AddInterface("IZeroMessage", name);

            // Remove
            classEditor.RemoveMethod("FromBytes");
            classEditor.RemoveMethod("ToBytes");

            var result = classEditor.GetTypeVariableDic();
            var keyList = result.Item1;
            var varList = result.Item2;

            StringBuilder code = new StringBuilder();

            #region [Generate WriteMethod By Variable]
            code.Append(@"

        public void FromBytes(byte[] src, ref int offset)
        {");
            for (int i = 0; i < keyList.Count; i++)
            {
                var typeStr = keyList[i];
                var variable = varList[i];
                string line = @"
            " + $"{variable} = {GetReadFuncByTypeStr(typeStr)};";
                code.Append(line);
            }

            code.AppendLine(@"
            offset += src.Length;
        }");

            // Add Method 
            MethodEditor methodEditor = new MethodEditor(code.ToString());
            classEditor.AddMethod(methodEditor);
            #endregion

            #region [Generate ToBytes By Variable]
            code.Clear();
            code.Append(@"

        public byte[] ToBytes()
        {");
            code.Append(@"
            int offset = 0;
            byte[] result = new byte[" + msgMaxSize + "];");
            for (int i = 0; i < keyList.Count; i++)
            {
                var typeStr = keyList[i];
                var variable = varList[i];
                string line = @"
            " + $"{GetWriteFuncByTypeStr(typeStr, variable)};";
                code.Append(line);
            }
            code.AppendLine(@"
            return result;
        }");

            // Add Method 
            methodEditor = new MethodEditor(code.ToString());
            classEditor.AddMethod(methodEditor);
            #endregion

            // Save File
            using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Write))
            {
                stream.SetLength(0);
                byte[] bytes = Encoding.UTF8.GetBytes(classEditor.Text.ToString());
                stream.Write(bytes, 0, bytes.Length);
            }

        }

        static string GetWriteFuncByTypeStr(string typeStr, string variable)
        {
            switch (typeStr.ToLower())
            {
                case "bool":
                    return $"BufferWriter.WriteBool(result, {variable}, ref offset)";
                case "bool[]":
                    return $"BufferWriter.WriteBoolArray(result, {variable}, ref offset)";
                case "byte":
                    return $"BufferWriter.WriteByte(result, {variable}, ref offset)";
                case "byte[]":
                    return $"BufferWriter.WriteByteArray(result, {variable}, ref offset)";
                case "sbyte":
                    return $"BufferWriter.WriteSByte(result, {variable}, ref offset)";
                case "sbyte[]":
                    return $"BufferWriter.WriteSByteArray(result, {variable}, ref offset)";
                case "char":
                    return $"BufferWriter.WriteChar(result, {variable}, ref offset)";
                case "char[]":
                    return $"BufferWriter.WriteCharArray(result, {variable}, ref offset)";
                case "string":
                    return $"BufferWriter.WriteUTF8String(result, {variable}, ref offset)";
                case "string[]":
                    return $"BufferWriter.WriteUTF8StringArray(result, {variable}, ref offset)";
                case "short":
                    return $"BufferWriter.WriteInt16(result, {variable}, ref offset)";
                case "short[]":
                    return $"BufferWriter.WriteInt16Array(result, {variable}, ref offset)";
                case "ushort":
                    return $"BufferWriter.WriteUInt16(result, {variable}, ref offset)";
                case "ushort[]":
                    return $"BufferWriter.WriteUInt16Array(result, {variable}, ref offset)";
                case "int":
                    return $"BufferWriter.WriteInt32(result, {variable}, ref offset)";
                case "int[]":
                    return $"BufferWriter.WriteInt32Array(result, {variable}, ref offset)";
                case "uint":
                    return $"BufferWriter.WriteUInt32(result, {variable}, ref offset)";
                case "uint[]":
                    return $"BufferWriter.WriteUInt32Array(result, {variable}, ref offset)";
                case "long":
                    return $"BufferWriter.WriteInt64(result, {variable}, ref offset)";
                case "long[]":
                    return $"BufferWriter.WriteInt64Array(result, {variable}, ref offset)";
                case "ulong":
                    return $"BufferWriter.WriteUInt64(result, {variable}, ref offset)";
                case "ulong[]":
                    return $"BufferWriter.WriteUInt64Array(result, {variable}, ref offset)";
                default:
                    return string.Empty;
            }
        }

        static string GetReadFuncByTypeStr(string typeStr)
        {
            switch (typeStr.ToLower())
            {
                case "bool":
                    return "BufferReader.ReadBool(src, ref offset)";
                case "bool[]":
                    return "BufferReader.ReadBoolArray(src, ref offset)";
                case "byte":
                    return "BufferReader.ReadByte(src, ref offset)";
                case "byte[]":
                    return "BufferReader.ReadByteArray(src, ref offset)";
                case "sbyte":
                    return "BufferReader.ReadSByte(src, ref offset)";
                case "sbyte[]":
                    return "BufferReader.ReadSByteArray(src, ref offset)";
                case "char":
                    return "BufferReader.ReadChar(src, ref offset)";
                case "char[]":
                    return "BufferReader.ReadCharArray(src, ref offset)";
                case "string":
                    return "BufferReader.ReadUTF8String(src, ref offset)";
                case "string[]":
                    return "BufferReader.ReadUTF8StringArray(src, ref offset)";
                case "short":
                    return "BufferReader.ReadInt16(src, ref offset)";
                case "short[]":
                    return "BufferReader.ReadInt16Array(src, ref offset)";
                case "ushort":
                    return "BufferReader.ReadUInt16(src, ref offset)";
                case "ushort[]":
                    return "BufferReader.ReadUInt16Array(src, ref offset)";
                case "int":
                    return "BufferReader.ReadInt32(src, ref offset)";
                case "int[]":
                    return "BufferReader.ReadInt32Array(src, ref offset)";
                case "uint":
                    return "BufferReader.ReadUInt32(src, ref offset)";
                case "uint[]":
                    return "BufferReader.ReadUInt32Array(src, ref offset)";
                case "long":
                    return "BufferReader.ReadInt64(src, ref offset)";
                case "long[]":
                    return "BufferReader.ReadInt64Array(src, ref offset)";
                case "ulong":
                    return "BufferReader.ReadUInt64(src, ref offset)";
                case "ulong[]":
                    return "BufferReader.ReadUInt64Array(src, ref offset)";
                default:
                    return string.Empty;
            }
        }

    }
}
