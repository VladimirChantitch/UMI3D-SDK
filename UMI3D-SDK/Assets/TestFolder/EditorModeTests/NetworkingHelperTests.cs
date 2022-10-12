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

        #region TryRead TESTs 
        #region True
        [Test]
        public void TryReadChar_Test()
        {
            char test_char = 'e';
            byte[] bytes = new byte[3];

            byteContainer = new ByteContainer(bytes);
            bool result = UMI3DNetworkingHelper.TryRead<char>(byteContainer, out test_char);
            Assert.True(result);
        }

        [Test]
        public void TryReadbool_Test()
        {
            bool test_bool = false;
            byte[] bytes = new byte[1];

            byteContainer = new ByteContainer(bytes);
            bool result = UMI3DNetworkingHelper.TryRead<bool>(byteContainer, out test_bool);
            Assert.True(result);
        }

        [Test]
        public void TryReadByte_Test()
        {
            byte test_byte = 1;
            byte[] bytes = new byte[1];

            byteContainer = new ByteContainer(bytes);
            bool result = UMI3DNetworkingHelper.TryRead<byte>(byteContainer, out test_byte);
            Assert.True(result);
        }

        [Test]
        public void TryReadShort_Test()
        {
            short test_short = 58;
            byte[] bytes = new byte[2];

            byteContainer = new ByteContainer(bytes);
            bool result = UMI3DNetworkingHelper.TryRead<short>(byteContainer, out test_short);
            Assert.True(result);
        }

        [Test]
        public void TryReadUShort_Test()
        {
            ushort test_ushort = 42;
            byte[] bytes = new byte[2];

            byteContainer = new ByteContainer(bytes);
            bool result = UMI3DNetworkingHelper.TryRead<ushort>(byteContainer, out test_ushort);
            Assert.True(result);
        }

        [Test]
        public void TryReadInt_Test()
        {
            int test_int = 42;
            byte[] bytes = new byte[4];

            byteContainer = new ByteContainer(bytes);
            bool result = UMI3DNetworkingHelper.TryRead<int>(byteContainer, out test_int);
            Assert.True(result);
        }
        [Test]
        public void TryReadUInt_Test()
        {
            uint test_uint = 42;
            byte[] bytes = new byte[4];

            byteContainer = new ByteContainer(bytes);
            bool result = UMI3DNetworkingHelper.TryRead<uint>(byteContainer, out test_uint);
            Assert.True(result);
        }
        [Test]
        public void TryReadFloat_Test()
        {
            float test_float = 42;
            byte[] bytes = new byte[4];

            byteContainer = new ByteContainer(bytes);
            bool result = UMI3DNetworkingHelper.TryRead<float>(byteContainer, out test_float);
            Assert.True(result);
        }
        [Test]
        public void TryReadLong_Test()
        {
            long test_long = 42;
            byte[] bytes = new byte[8];

            byteContainer = new ByteContainer(bytes);
            bool result = UMI3DNetworkingHelper.TryRead<long>(byteContainer, out test_long);
            Assert.True(result);
        }
        [Test]
        public void TryReadULong_Test()
        {
            ulong test_ulong = 42;
            byte[] bytes = new byte[8];

            byteContainer = new ByteContainer(bytes);
            bool result = UMI3DNetworkingHelper.TryRead<ulong>(byteContainer, out test_ulong);
            Assert.True(result);
        }
        [Test]
        public void TryReadSerializableVector2_Test()
        {
            SerializableVector2 test_serializableVector2 = new SerializableVector2();
            byte[] bytes = new byte[8];

            byteContainer = new ByteContainer(bytes);
            bool result = UMI3DNetworkingHelper.TryRead<SerializableVector2>(byteContainer, out test_serializableVector2);
            Assert.True(result);
        }
        [Test]
        public void TryReadVector2_Test()
        {
            Vector2 test_Vector2 = new Vector2();
            byte[] bytes = new byte[8];

            byteContainer = new ByteContainer(bytes);
            bool result = UMI3DNetworkingHelper.TryRead<Vector2>(byteContainer, out test_Vector2);
            Assert.True(result);
        }
        [Test]
        public void TryReadVector3_Test()
        {
            Vector3 test_Vector3 = new Vector3();
            byte[] bytes = new byte[12];

            byteContainer = new ByteContainer(bytes);
            bool result = UMI3DNetworkingHelper.TryRead<Vector3>(byteContainer, out test_Vector3);
            Assert.True(result);
        }
        [Test]
        public void TryReadQuaternion_Test()
        {
            Quaternion test_Quaternion = new Quaternion();
            byte[] bytes = new byte[16];

            byteContainer = new ByteContainer(bytes);
            bool result = UMI3DNetworkingHelper.TryRead<Quaternion>(byteContainer, out test_Quaternion);
            Assert.True(result);
        }
        [Test]
        public void TryReadSerializableColor_Test()
        {
            SerializableColor test_SerializableColor = new SerializableColor();
            byte[] bytes = new byte[16];

            byteContainer = new ByteContainer(bytes);
            bool result = UMI3DNetworkingHelper.TryRead<SerializableColor>(byteContainer, out test_SerializableColor);
            Assert.True(result);
        }
        [Test]
        public void TryReadColor_Test()
        {
            Color test_Color = new Color();
            byte[] bytes = new byte[16];

            byteContainer = new ByteContainer(bytes);
            bool result = UMI3DNetworkingHelper.TryRead<Color>(byteContainer, out test_Color);
            Assert.True(result);
        }
        [Test]
        public void TryReadSerializableVector4_Test()
        {
            SerializableVector4 test_SerializableVector4 = new SerializableVector4();
            byte[] bytes = new byte[16];

            byteContainer = new ByteContainer(bytes);
            bool result = UMI3DNetworkingHelper.TryRead<SerializableVector4>(byteContainer, out test_SerializableVector4);
            Assert.True(result);
        }
        [Test]
        public void TryReadVector4_Test()
        {
            Vector4 test_Vector4 = new Vector4();
            byte[] bytes = new byte[16];

            byteContainer = new ByteContainer(bytes);
            bool result = UMI3DNetworkingHelper.TryRead<Vector4>(byteContainer, out test_Vector4);
            Assert.True(result);
        }
        [Test]
        public void TryReadSerializableMatrix4x4_Test()
        {
            SerializableMatrix4x4 test_SerializableMatrix4x4 = new SerializableMatrix4x4();
            byte[] bytes = new byte[64];

            byteContainer = new ByteContainer(bytes);
            bool result = UMI3DNetworkingHelper.TryRead<SerializableMatrix4x4>(byteContainer, out test_SerializableMatrix4x4);
            Assert.True(result);
        }

        [Test]
        public void TryReadUMI3DShaderPropertyDto_Test()
        {
            UMI3DShaderPropertyDto test_UMI3DShaderPropertyDto = new UMI3DShaderPropertyDto(new object());
            byte[] bytes = new byte[8];

            Debug.Log("this test should'nt work");

            byteContainer = new ByteContainer(bytes);
            bool result = UMI3DNetworkingHelper.TryRead<UMI3DShaderPropertyDto>(byteContainer, out test_UMI3DShaderPropertyDto);
            Assert.True(result);
        }
        #endregion
        #region False
        [Test]
        public void TryReadUMI3DShaderPropertyDto_False_Test()
        {
            UMI3DShaderPropertyDto test_UMI3DShaderPropertyDto = new UMI3DShaderPropertyDto(new object());
            byte[] bytes = new byte[0];

            Debug.Log("this test should'nt work");

            byteContainer = new ByteContainer(bytes);
            bool result = UMI3DNetworkingHelper.TryRead<UMI3DShaderPropertyDto>(byteContainer, out test_UMI3DShaderPropertyDto);
            Assert.False(result);
        }
        [Test]
        public void TryReadNullContainer_Test()
        {
            char test_char = 'e';
            bool result = UMI3DNetworkingHelper.TryRead<char>(byteContainer, out test_char);
            Assert.False(result);
        }
        #endregion
        #endregion
    }
}
