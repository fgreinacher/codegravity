﻿// Type: ClrTest.Reflection.InlineMethodInstruction
// Assembly: Depender.Parser, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7BFB306E-D2F0-493B-8C2F-5C3CEA04F731
// Assembly location: D:\temp\NSplit\Depender\Depender.Parser.dll

using System.Reflection;
using System.Reflection.Emit;

namespace ClrTest.Reflection
{
  public class InlineMethodInstruction : ILInstruction
  {
    private ITokenResolver m_resolver;
    private int m_token;
    private MethodBase m_method;

    public MethodBase Method
    {
      get
      {
        if (this.m_method == null)
          this.m_method = this.m_resolver.AsMethod(this.m_token);
        return this.m_method;
      }
    }

    public int Token
    {
      get
      {
        return this.m_token;
      }
    }

    internal InlineMethodInstruction(int offset, OpCode opCode, int token, ITokenResolver resolver)
      : base(offset, opCode)
    {
      this.m_resolver = resolver;
      this.m_token = token;
    }

    public override void Accept(ILInstructionVisitor vistor)
    {
      vistor.VisitInlineMethodInstruction(this);
    }
  }
}
