using Xunit;
using AwesomeBooks.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using AwesomeBooks.Data;
using AwesomeBooks.Exceptions;

namespace AwesomeBooks.Repositories.Tests
{
    public class InventoryRepositoryTests
    {
        private Mock<IJsonContext> _jsonContextMock;

        public InventoryRepositoryTests()
        {
            _jsonContextMock = new Mock<IJsonContext>();
        }

    }
}