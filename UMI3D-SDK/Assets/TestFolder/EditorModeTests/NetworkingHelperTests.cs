using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using umi3d.common;
using System.Linq;
using System;
using System.Security;

namespace Tests
{
    public class NetworkingHelperTests : MonoBehaviour
    {
        ByteContainer byteContainer;
        [SetUp]
        public void SetUp()
        {

        }

        [TearDown]
        public void TearDown()
        {
            byteContainer = null;
        }

        [Test]
        public void TryReadNullContainer_Test()
        {
            char test_char = 'e';
            bool result = UMI3DNetworkingHelper.TryRead<char>(byteContainer, out test_char);
            Assert.False(result);
        }
        [Test]
        public void TryReadChar_Test()
        {
            char test_char = 'e';
            byte[] bytes = new byte[3];
            bytes[0] = 1;
            bytes[1] = 0;
            bytes[2] = 1;

            byteContainer = new ByteContainer(bytes);
            bool result = UMI3DNetworkingHelper.TryRead<char>(byteContainer, out test_char);
            Assert.True(result);
        }

        [Test]
        public void TryReadbool_Test()
        {
            bool test_bool = false;
            byte[] bytes = new byte[1];
            bytes[0] = 1;

            byteContainer = new ByteContainer(bytes);
            bool result = UMI3DNetworkingHelper.TryRead<bool>(byteContainer, out test_bool);
            Assert.True(result);
        }
    }
}
