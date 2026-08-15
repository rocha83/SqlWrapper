using System;
using System.Collections.Generic;
using System.Reflection;
using Xunit;
using Rochas.SqlWrapper.Helpers;

namespace Rochas.SqlWrapper.Test
{
    public class OrderLine
    {
        public decimal Id { get; set; }
        public string Product { get; set; } = "";
        public int Quantity { get; set; }
    }

    public class Order
    {
        public decimal Id { get; set; }
        public string Description { get; set; } = "";
        public List<OrderLine> Lines { get; set; } = new List<OrderLine>();
        public Credential Metadata { get; set; } = new Credential();
    }

    public class Credential
    {
        public decimal Id { get; set; }
        public string Logon { get; set; } = "";
        public string TokenId { get; set; } = "";
    }

    public class Employee
    {
        public decimal Id { get; set; }
        public string Name { get; set; } = "";
        public decimal Age { get; set; }
        public bool Active { get; set; }
        public Credential Credential { get; set; } = new Credential();
    }

    public class Person
    {
        public decimal Id { get; set; }
        public string Name { get; set; } = "";
        public string City { get; set; } = "";
        public string State { get; set; } = "";
        public decimal Age { get; set; }
        public bool Active { get; set; }
        public decimal CreditLimit { get; set; }
    }

    public class ReflectorMigrationTests
    {
        private readonly List<Person> _testData;

        public ReflectorMigrationTests()
        {
            _testData = new List<Person>
            {
                new Person { Id = 1, Name = "Carlos Silva", City = "São Paulo", State = "SP", Age = 35, Active = true, CreditLimit = 5000 },
                new Person { Id = 2, Name = "Ana Oliveira", City = "Rio de Janeiro", State = "RJ", Age = 28, Active = true, CreditLimit = 3000 },
            };
        }

        private List<Employee> _employeeData()
        {
            return new List<Employee>
            {
                new Employee { Id = 1, Name = "Carlos Silva", Age = 35, Active = true, Credential = new Credential { Logon = "carlos" } },
            };
        }

        #region Reflection helpers (migrated from BWOQ)

        [Fact]
        public void InitNullComposition_InitializesChildInstances()
        {
            var emp = new Employee { Credential = null };
            EntityReflector.InitNullComposition(emp);

            Assert.NotNull(emp.Credential);
        }

        [Fact]
        public void InitNullComposition_NullSource_IsNoOp()
        {
            EntityReflector.InitNullComposition(null);
        }

        [Fact]
        public void CloneObjectData_CopiesScalarProperties()
        {
            var source = _testData[0];
            var dest = new Person();

            EntityReflector.CloneObjectData(source, dest);

            Assert.Equal("Carlos Silva", dest.Name);
            Assert.Equal("São Paulo", dest.City);
            Assert.Equal(35m, dest.Age);
        }

        [Fact]
        public void CloneObjectData_NullDestination_CreatesInstance()
        {
            var source = _testData[0];

            EntityReflector.CloneObjectData(source, null);
        }

        [Fact]
        public void CloneObjectData_DeepClonesListsAndCompositions()
        {
            var source = new Order
            {
                Id = 10,
                Description = "Pedido",
                Lines = new List<OrderLine>
                {
                    new OrderLine { Id = 1, Product = "A", Quantity = 2 },
                    new OrderLine { Id = 2, Product = "B", Quantity = 3 },
                },
                Metadata = new Credential { Logon = "ops.logon" },
            };
            var dest = new Order();

            EntityReflector.CloneObjectData(source, dest);

            Assert.Equal("Pedido", dest.Description);
            Assert.Equal(2, dest.Lines.Count);
            Assert.Equal("B", dest.Lines[1].Product);
            Assert.Equal("ops.logon", dest.Metadata.Logon);
        }

        [Fact]
        public void GetObjectProps_ReturnsAllPublicProps()
        {
            var props = EntityReflector.GetObjectProps(_testData[0]);

            Assert.Equal(7, props.Length);
        }

        [Fact]
        public void GetObjectProps_FilterByName()
        {
            var props = EntityReflector.GetObjectProps(_testData[0], "Name", "City");

            Assert.Equal(2, props.Length);
            Assert.Contains(props, p => p.Name == "Name");
        }

        [Fact]
        public void GetObjectProps_ChildNavigationFilter()
        {
            var props = EntityReflector.GetObjectProps(_employeeData()[0], "Credential.Logon");

            Assert.Equal(1, props.Length);
            Assert.Equal("Logon", props[0].Name);
        }

        [Fact]
        public void GetObjectProps_ChildFilterWithNullInstance()
        {
            var props = EntityReflector.GetObjectProps(new Employee { Credential = null }, "Credential.Logon");

            Assert.Equal(1, props.Length);
            Assert.Equal("Logon", props[0].Name);
        }

        [Fact]
        public void GetObjectProps_UnknownFilter_ReturnsEmpty()
        {
            var props = EntityReflector.GetObjectProps(_testData[0], "NotARealProp");

            Assert.Empty(props);
        }

        [Fact]
        public void GetObjectPropValues_ReturnsValuesArray()
        {
            var props = EntityReflector.GetObjectProps(_testData[0], "Name", "Age");
            var values = EntityReflector.GetObjectPropValues(_testData[0], props);

            Assert.Equal("Carlos Silva", values[0]);
            Assert.Equal(35m, values[1]);
        }

        [Fact]
        public void GetTypedValue_CoversAllScalarTypes()
        {
            Assert.Null(EntityReflector.GetTypedValue(typeof(string), null));
            Assert.Null(EntityReflector.GetTypedValue(typeof(string), DBNull.Value));
            Assert.Equal((short)5, EntityReflector.GetTypedValue(typeof(short), "5"));
            Assert.Equal(5, EntityReflector.GetTypedValue(typeof(int), "5"));
            Assert.Equal(5L, EntityReflector.GetTypedValue(typeof(long), "5"));
            Assert.Equal(5m, EntityReflector.GetTypedValue(typeof(decimal), "5"));
            Assert.Equal(5d, EntityReflector.GetTypedValue(typeof(double), "5"));
            Assert.Equal(5f, EntityReflector.GetTypedValue(typeof(float), "5"));
            Assert.Equal(5f, EntityReflector.GetTypedValue(typeof(Single), "5"));
            Assert.True((bool)EntityReflector.GetTypedValue(typeof(bool), "true"));
            Assert.Equal("abc", EntityReflector.GetTypedValue(typeof(string), "abc"));
            Assert.Equal(new DateTime(2020, 5, 1), EntityReflector.GetTypedValue(typeof(DateTime), "2020-05-01"));
            Assert.Equal(42, EntityReflector.GetTypedValue(typeof(object), 42));
        }

        [Fact]
        public void GetObjectChilds_ReturnsChildInstances()
        {
            var method = typeof(EntityReflector).GetMethod("getObjectChilds",
                BindingFlags.Static | BindingFlags.NonPublic);
            var result = (object[])method.Invoke(null, new object[] { new Employee() });

            Assert.NotNull(result);
            Assert.Contains(result, r => r is Credential);
        }

        [Fact]
        public void CloneObjectDataGeneric_ReturnsTypedClone()
        {
            var clone = EntityReflector.CloneObjectData<Person>(_testData[0]);

            Assert.Equal("Carlos Silva", clone.Name);
            Assert.Equal(5000m, clone.CreditLimit);
        }

        #endregion
    }
}
