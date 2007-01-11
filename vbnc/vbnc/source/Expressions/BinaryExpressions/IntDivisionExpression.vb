' 
' Visual Basic.Net Compiler
' Copyright (C) 2004 - 2007 Rolf Bjarne Kvinge, RKvinge@novell.com
' 
' This library is free software; you can redistribute it and/or
' modify it under the terms of the GNU Lesser General Public
' License as published by the Free Software Foundation; either
' version 2.1 of the License, or (at your option) any later version.
' 
' This library is distributed in the hope that it will be useful,
' but WITHOUT ANY WARRANTY; without even the implied warranty of
' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
' Lesser General Public License for more details.
' 
' You should have received a copy of the GNU Lesser General Public
' License along with this library; if not, write to the Free Software
' Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
' 

Public Class IntDivisionExpression
    Inherits BinaryExpression

    Protected Overrides Function GenerateCodeInternal(ByVal Info As EmitInfo) As Boolean
        Dim result As Boolean = True

        ValidateBeforeGenerateCode(Info)

        Dim expInfo As EmitInfo = Info.Clone(True, False, OperandType)

        result = m_LeftExpression.GenerateCode(expInfo) AndAlso result
        result = m_RightExpression.GenerateCode(expInfo) AndAlso result

        Select Case OperandTypeCode
            Case TypeCode.SByte, TypeCode.Byte, TypeCode.Int16, TypeCode.UInt16, TypeCode.Int32, TypeCode.UInt32, TypeCode.Int64, TypeCode.UInt64
                Emitter.EmitIntDiv(Info, OperandType)
            Case TypeCode.Object
                Helper.Assert(Helper.CompareType(OperandType, Compiler.TypeCache.Object))
                Emitter.EmitCall(Info, Compiler.TypeCache.MS_VB_CS_Operators_IntDivideObject__Object_Object)
            Case Else
                Throw New InternalException(Me)
        End Select

        Return result
    End Function

    Sub New(ByVal Parent As ParsedObject, ByVal LExp As Expression, ByVal RExp As Expression)
        MyBase.New(Parent, LExp, RExp)
    End Sub

    Public Overrides ReadOnly Property Keyword() As KS
        Get
            Return KS.IntDivision
        End Get
    End Property

    Public Overrides ReadOnly Property IsConstant() As Boolean
        Get
            Return MyBase.IsConstant 'CHECK: is this true?
        End Get
    End Property

    Public Overrides ReadOnly Property ConstantValue() As Object
        Get
            Dim rvalue, lvalue As Object
            lvalue = m_LeftExpression.ConstantValue
            rvalue = m_RightExpression.ConstantValue
            If lvalue Is Nothing Or rvalue Is Nothing Then
                Return Nothing
            Else

                Dim tlvalue, trvalue As Type
                Dim clvalue, crvalue As TypeCode
                tlvalue = lvalue.GetType
                clvalue = Helper.GetTypeCode(tlvalue)
                trvalue = rvalue.GetType
                crvalue = Helper.GetTypeCode(trvalue)

                Dim smallest As Type
                Dim csmallest As TypeCode
                smallest = Compiler.TypeResolution.GetSmallestIntegralType(tlvalue, trvalue)
                Helper.Assert(smallest IsNot Nothing)
                csmallest = Helper.GetTypeCode(smallest)
                If CDbl(rvalue) = 0 Then
                    Helper.Stop() 'TODO: Adderror (Divide by zero).
                End If

                Select Case csmallest
                    Case TypeCode.Byte
                        Return CByte(lvalue) \ CByte(rvalue)
                    Case TypeCode.SByte
                        If CSByte(lvalue) = SByte.MinValue AndAlso CSByte(rvalue) = -1 Then
                            Return CShort(lvalue) \ CShort(rvalue)
                        Else
                            Return CSByte(lvalue) \ CSByte(rvalue)
                        End If
                    Case TypeCode.Int16
                        If CShort(lvalue) = Short.MinValue AndAlso CShort(rvalue) = -1 Then
                            Return CInt(lvalue) \ CInt(rvalue)
                        Else
                            Return CShort(lvalue) \ CShort(rvalue)
                        End If
                    Case TypeCode.UInt16
                        Return CUShort(lvalue) \ CUShort(rvalue)
                    Case TypeCode.Int32
                        If CInt(lvalue) = Integer.MinValue AndAlso CInt(rvalue) = -1 Then
                            Return CLng(lvalue) \ CLng(rvalue)
                        Else
                            Return CInt(lvalue) \ CInt(rvalue)
                        End If
                    Case TypeCode.UInt32
                        Return CUInt(lvalue) / CUInt(rvalue)
                    Case TypeCode.Int64
                        If CLng(lvalue) = Long.MinValue AndAlso CLng(rvalue) = -1 Then
                            'Return CDec(lvalue) \ CDec(rvalue)
                            Throw New InternalException(Me)
                        Else
                            Return CLng(lvalue) \ CLng(rvalue)
                        End If
                    Case TypeCode.UInt64
                        Return CULng(lvalue) \ CULng(rvalue)
                    Case TypeCode.Double
                        'Return CDbl(lvalue) \ CDbl(rvalue)
                        Throw New InternalException(Me)
                    Case TypeCode.Single
                        'Return CSng(lvalue) \ CSng(rvalue)
                        Throw New InternalException(Me)
                    Case TypeCode.Decimal
                        'Return CDec(lvalue) \ CDec(rvalue)
                        Throw New InternalException(Me)
                    Case Else
                        Helper.Stop()
                        Throw New InternalException(Me)
                End Select
            End If
        End Get
    End Property
End Class