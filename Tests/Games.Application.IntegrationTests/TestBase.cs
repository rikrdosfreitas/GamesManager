﻿using System.Threading.Tasks;
using NUnit.Framework;

namespace Games.Application.IntegrationTests
{
    using static Testing;

    public class TestBase
    {
        [SetUp]
        public async Task TestSetUp()
        {
            await ResetState();

        }
    }
}