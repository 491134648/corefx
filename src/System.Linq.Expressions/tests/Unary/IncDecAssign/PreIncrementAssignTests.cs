// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Reflection;
using Xunit;

namespace System.Linq.Expressions.Tests
{
    public class PreIncrementAssignTests : IncDecAssignTests
    {
        [Theory]
        [PerCompilationType(nameof(Int16sAndIncrements))]
        [PerCompilationType(nameof(NullableInt16sAndIncrements))]
        [PerCompilationType(nameof(UInt16sAndIncrements))]
        [PerCompilationType(nameof(NullableUInt16sAndIncrements))]
        [PerCompilationType(nameof(Int32sAndIncrements))]
        [PerCompilationType(nameof(NullableInt32sAndIncrements))]
        [PerCompilationType(nameof(UInt32sAndIncrements))]
        [PerCompilationType(nameof(NullableUInt32sAndIncrements))]
        [PerCompilationType(nameof(Int64sAndIncrements))]
        [PerCompilationType(nameof(NullableInt64sAndIncrements))]
        [PerCompilationType(nameof(UInt64sAndIncrements))]
        [PerCompilationType(nameof(NullableUInt64sAndIncrements))]
        [PerCompilationType(nameof(DecimalsAndIncrements))]
        [PerCompilationType(nameof(NullableDecimalsAndIncrements))]
        [PerCompilationType(nameof(SinglesAndIncrements))]
        [PerCompilationType(nameof(NullableSinglesAndIncrements))]
        [PerCompilationType(nameof(DoublesAndIncrements))]
        [PerCompilationType(nameof(NullableDoublesAndIncrements))]
        public void ReturnsCorrectValues(Type type, object value, object result, bool useInterpreter)
        {
            ParameterExpression variable = Expression.Variable(type);
            BlockExpression block = Expression.Block(
                new[] { variable },
                Expression.Assign(variable, Expression.Constant(value, type)),
                Expression.PreIncrementAssign(variable)
                );
            Assert.True(Expression.Lambda<Func<bool>>(Expression.Equal(Expression.Constant(result, type), block)).Compile(useInterpreter)());
        }

        [Theory]
        [PerCompilationType(nameof(Int16sAndIncrements))]
        [PerCompilationType(nameof(NullableInt16sAndIncrements))]
        [PerCompilationType(nameof(UInt16sAndIncrements))]
        [PerCompilationType(nameof(NullableUInt16sAndIncrements))]
        [PerCompilationType(nameof(Int32sAndIncrements))]
        [PerCompilationType(nameof(NullableInt32sAndIncrements))]
        [PerCompilationType(nameof(UInt32sAndIncrements))]
        [PerCompilationType(nameof(NullableUInt32sAndIncrements))]
        [PerCompilationType(nameof(Int64sAndIncrements))]
        [PerCompilationType(nameof(NullableInt64sAndIncrements))]
        [PerCompilationType(nameof(UInt64sAndIncrements))]
        [PerCompilationType(nameof(NullableUInt64sAndIncrements))]
        [PerCompilationType(nameof(DecimalsAndIncrements))]
        [PerCompilationType(nameof(NullableDecimalsAndIncrements))]
        [PerCompilationType(nameof(SinglesAndIncrements))]
        [PerCompilationType(nameof(NullableSinglesAndIncrements))]
        [PerCompilationType(nameof(DoublesAndIncrements))]
        [PerCompilationType(nameof(NullableDoublesAndIncrements))]
        public void AssignsCorrectValues(Type type, object value, object result, bool useInterpreter)
        {
            ParameterExpression variable = Expression.Variable(type);
            LabelTarget target = Expression.Label(type);
            BlockExpression block = Expression.Block(
                new[] { variable },
                Expression.Assign(variable, Expression.Constant(value, type)),
                Expression.PreIncrementAssign(variable),
                Expression.Return(target, variable),
                Expression.Label(target, Expression.Default(type))
                );
            Assert.True(Expression.Lambda<Func<bool>>(Expression.Equal(Expression.Constant(result, type), block)).Compile(useInterpreter)());
        }

        [Theory]
        [ClassData(typeof(CompilationTypes))]
        public void SingleNanToNan(bool useInterpreter)
        {
            TestPropertyClass<float> instance = new TestPropertyClass<float>();
            instance.TestInstance = float.NaN;
            Assert.True(float.IsNaN(
                Expression.Lambda<Func<float>>(
                    Expression.PreIncrementAssign(
                        Expression.Property(
                            Expression.Constant(instance),
                            typeof(TestPropertyClass<float>),
                            "TestInstance"
                            )
                        )
                    ).Compile(useInterpreter)()
                ));
            Assert.True(float.IsNaN(instance.TestInstance));
        }

        [Theory]
        [ClassData(typeof(CompilationTypes))]
        public void DoubleNanToNan(bool useInterpreter)
        {
            TestPropertyClass<double> instance = new TestPropertyClass<double>();
            instance.TestInstance = double.NaN;
            Assert.True(double.IsNaN(
                Expression.Lambda<Func<double>>(
                    Expression.PreIncrementAssign(
                        Expression.Property(
                            Expression.Constant(instance),
                            typeof(TestPropertyClass<double>),
                            "TestInstance"
                            )
                        )
                    ).Compile(useInterpreter)()
                ));
            Assert.True(double.IsNaN(instance.TestInstance));
        }

        [Theory]
        [PerCompilationType(nameof(IncrementOverflowingValues))]
        public void OverflowingValuesThrow(object value, bool useInterpreter)
        {
            ParameterExpression variable = Expression.Variable(value.GetType());
            Action overflow = Expression.Lambda<Action>(
                Expression.Block(
                    typeof(void),
                    new[] { variable },
                    Expression.Assign(variable, Expression.Constant(value)),
                    Expression.PreIncrementAssign(variable)
                    )
                ).Compile(useInterpreter);
            Assert.Throws<OverflowException>(overflow);
        }

        [Theory]
        [MemberData(nameof(UnincrementableAndUndecrementableTypes))]
        public void InvalidOperandType(Type type)
        {
            ParameterExpression variable = Expression.Variable(type);
            Assert.Throws<InvalidOperationException>(() => Expression.PreIncrementAssign(variable));
        }

        [Theory]
        [ClassData(typeof(CompilationTypes))]
        public void MethodCorrectResult(bool useInterpreter)
        {
            ParameterExpression variable = Expression.Variable(typeof(string));
            BlockExpression block = Expression.Block(
                new[] { variable },
                Expression.Assign(variable, Expression.Constant("hello")),
                Expression.PreIncrementAssign(variable, typeof(IncDecAssignTests).GetTypeInfo().GetDeclaredMethod("SillyMethod"))
                );
            Assert.Equal("Eggplant", Expression.Lambda<Func<string>>(block).Compile(useInterpreter)());
        }

        [Theory]
        [ClassData(typeof(CompilationTypes))]
        public void MethodCorrectAssign(bool useInterpreter)
        {
            ParameterExpression variable = Expression.Variable(typeof(string));
            LabelTarget target = Expression.Label(typeof(string));
            BlockExpression block = Expression.Block(
                new[] { variable },
                Expression.Assign(variable, Expression.Constant("hello")),
                Expression.PreIncrementAssign(variable, typeof(IncDecAssignTests).GetTypeInfo().GetDeclaredMethod("SillyMethod")),
                Expression.Return(target, variable),
                Expression.Label(target, Expression.Default(typeof(string)))
                );
            Assert.Equal("Eggplant", Expression.Lambda<Func<string>>(block).Compile(useInterpreter)());
        }

        [Fact]
        public void IncorrectMethodType()
        {
            Expression variable = Expression.Variable(typeof(int));
            MethodInfo method = typeof(IncDecAssignTests).GetTypeInfo().GetDeclaredMethod("SillyMethod");
            Assert.Throws<InvalidOperationException>(() => Expression.PreIncrementAssign(variable, method));
        }

        [Fact]
        public void IncorrectMethodParameterCount()
        {
            Expression variable = Expression.Variable(typeof(string));
            MethodInfo method = typeof(object).GetTypeInfo().GetDeclaredMethod("ReferenceEquals");
            Assert.Throws<ArgumentException>("method", () => Expression.PreIncrementAssign(variable, method));
        }

        [Fact]
        public void IncorrectMethodReturnType()
        {
            Expression variable = Expression.Variable(typeof(int));
            MethodInfo method = typeof(IncDecAssignTests).GetTypeInfo().GetDeclaredMethod("GetString");
            Assert.Throws<ArgumentException>(null, () => Expression.PreIncrementAssign(variable, method));
        }

        [Theory]
        [ClassData(typeof(CompilationTypes))]
        public void StaticMemberAccessCorrect(bool useInterpreter)
        {
            TestPropertyClass<uint>.TestStatic = 2U;
            Assert.Equal(
                3U,
                Expression.Lambda<Func<uint>>(
                    Expression.PreIncrementAssign(
                        Expression.Property(null, typeof(TestPropertyClass<uint>), "TestStatic")
                        )
                    ).Compile(useInterpreter)()
                );
            Assert.Equal(3U, TestPropertyClass<uint>.TestStatic);
        }

        [Theory]
        [ClassData(typeof(CompilationTypes))]
        public void InstanceMemberAccessCorrect(bool useInterpreter)
        {
            TestPropertyClass<int> instance = new TestPropertyClass<int>();
            instance.TestInstance = 2;
            Assert.Equal(
                3,
                Expression.Lambda<Func<int>>(
                    Expression.PreIncrementAssign(
                        Expression.Property(
                            Expression.Constant(instance),
                            typeof(TestPropertyClass<int>),
                            "TestInstance"
                            )
                        )
                    ).Compile(useInterpreter)()
                );
            Assert.Equal(3, instance.TestInstance);
        }

        [Theory]
        [ClassData(typeof(CompilationTypes))]
        public void ArrayAccessCorrect(bool useInterpreter)
        {
            int[] array = new int[1];
            array[0] = 2;
            Assert.Equal(
                3,
                Expression.Lambda<Func<int>>(
                    Expression.PreIncrementAssign(
                        Expression.ArrayAccess(Expression.Constant(array), Expression.Constant(0))
                        )
                    ).Compile(useInterpreter)()
                );
            Assert.Equal(3, array[0]);
        }

        [Fact]
        public void CanReduce()
        {
            ParameterExpression variable = Expression.Variable(typeof(int));
            UnaryExpression op = Expression.PreIncrementAssign(variable);
            Assert.True(op.CanReduce);
            Assert.NotSame(op, op.ReduceAndCheck());
        }

        [Fact]
        public void NullOperand()
        {
            Assert.Throws<ArgumentNullException>("expression", () => Expression.PreIncrementAssign(null));
        }

        [Fact]
        public void UnwritableOperand()
        {
            Assert.Throws<ArgumentException>("expression", () => Expression.PreIncrementAssign(Expression.Constant(1)));
        }

        [Fact]
        public void UnreadableOperand()
        {
            Expression value = Expression.Property(null, typeof(Unreadable<int>), "WriteOnly");
            Assert.Throws<ArgumentException>("expression", () => Expression.PreIncrementAssign(value));
        }

        [Fact]
        public void UpdateSameOperandSameNode()
        {
            UnaryExpression op = Expression.PreIncrementAssign(Expression.Variable(typeof(int)));
            Assert.Same(op, op.Update(op.Operand));
            Assert.Same(op, NoOpVisitor.Instance.Visit(op));
        }

        [Fact]
        public void UpdateDiffOperandDiffNode()
        {
            UnaryExpression op = Expression.PreIncrementAssign(Expression.Variable(typeof(int)));
            Assert.NotSame(op, op.Update(Expression.Variable(typeof(int))));
        }

        [Fact]
        public void ToStringTest()
        {
            var e = Expression.PreIncrementAssign(Expression.Parameter(typeof(int), "x"));
            Assert.Equal("++x", e.ToString());
        }
    }
}
