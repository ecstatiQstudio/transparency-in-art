using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using SharpGLTF.Schema2;
using StbImageSharp;
using System.Linq;

namespace Classes
{
    public class GltfLoader
    {
        public GltfLoader(string path, float targetSize, float xRotationDegrees, float yRotationDegrees, float zRotationDegrees)
        {
            _path = path;
            _targetSize = targetSize;
            _xRotationDegrees = xRotationDegrees;
            _yRotationDegrees = yRotationDegrees;
            _zRotationDegrees = zRotationDegrees;
            _meshes = new List<Mesh>();
            _x = float.MaxValue;
            _y = float.MaxValue;
            _z = float.MaxValue;

            LoadModel();
        }

        public List<Classes.Mesh> Meshes { get { return _meshes; } }

        private System.Numerics.Vector4 GetColorParameterValue(SharpGLTF.Schema2.MeshPrimitive meshPrimitive)
        {
            System.Numerics.Vector4 result = System.Numerics.Vector4.One;

            SharpGLTF.Schema2.MaterialChannel? colorChannel = meshPrimitive.Material?.FindChannel(CHANNEL_KEY_BASE_COLOR);

            if (colorChannel != null)
            {
                result = colorChannel.Value.Color;
            }

            return result;
        }

        private float GetMetallicRoughnessParameterValue(SharpGLTF.Schema2.MeshPrimitive meshPrimitive, string name)
        {
            float result = 1f;

            SharpGLTF.Schema2.MaterialChannel? metallicRoughnessChannel = meshPrimitive.Material?.FindChannel(CHANNEL_KEY_METALLIC_ROUGHNESS);

            if (metallicRoughnessChannel != null)
            {
                foreach (SharpGLTF.Schema2.IMaterialParameter materialParameter in metallicRoughnessChannel.Value.Parameters)
                {
                    if (string.Compare(materialParameter.Name, name) == 0)
                    {
                        result = Convert.ToSingle(materialParameter.Value);
                        break;
                    }
                }
            }

            return result;
        }

        public static int CreateGenericTexture()
        {
            int handle = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, handle);

            byte[] pixel = { 255, 255, 255, 255 };

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, 1, 1, 0, PixelFormat.Rgba, PixelType.UnsignedByte, pixel);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, ((int)TextureMinFilter.Nearest));
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, ((int)TextureMagFilter.Nearest));
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, ((int)OpenTK.Graphics.OpenGL4.TextureWrapMode.Repeat));
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, ((int)OpenTK.Graphics.OpenGL4.TextureWrapMode.Repeat));

            return handle;
        }

        private int LoadImage(SharpGLTF.Schema2.Image image, bool isSrgb)
        {
            if (image == null
                || image.Content.IsEmpty)
            {
                return CreateGenericTexture();
            }

            int result = GL.GenTexture();

            GL.BindTexture(OpenTK.Graphics.OpenGL4.TextureTarget.Texture2D, result);

            byte[] bytes = image.Content.Content.ToArray();
            ImageResult imageResult = ImageResult.FromMemory(bytes, ColorComponents.Default);
            bool hasAlpha = imageResult.Comp == ColorComponents.RedGreenBlueAlpha;

            PixelInternalFormat pixelInternalFormat;
            PixelFormat pixelFormat;

            if (hasAlpha)
            {
                pixelInternalFormat = isSrgb ? PixelInternalFormat.SrgbAlpha : PixelInternalFormat.Rgba;
                pixelFormat = PixelFormat.Rgba;
            }
            else
            {
                pixelInternalFormat = isSrgb ? PixelInternalFormat.Srgb : PixelInternalFormat.Rgb;
                pixelFormat = PixelFormat.Rgb;
            }

            GL.TexImage2D(OpenTK.Graphics.OpenGL4.TextureTarget.Texture2D, 0, pixelInternalFormat, imageResult.Width, imageResult.Height, 0, pixelFormat, PixelType.UnsignedByte, imageResult.Data);
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
            GL.TexParameter(OpenTK.Graphics.OpenGL4.TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, ((int)TextureMinFilter.LinearMipmapLinear));
            GL.TexParameter(OpenTK.Graphics.OpenGL4.TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, ((int)TextureMagFilter.Linear));
            GL.TexParameter(OpenTK.Graphics.OpenGL4.TextureTarget.Texture2D, TextureParameterName.TextureWrapS, ((int)OpenTK.Graphics.OpenGL4.TextureWrapMode.ClampToEdge));
            GL.TexParameter(OpenTK.Graphics.OpenGL4.TextureTarget.Texture2D, TextureParameterName.TextureWrapT, ((int)OpenTK.Graphics.OpenGL4.TextureWrapMode.ClampToEdge));

            return result;
        }

        private int LoadTexture(string channelKey, SharpGLTF.Schema2.Material material)
        {
            if (material == null)
            {
                return CreateGenericTexture();
            }

            SharpGLTF.Schema2.MaterialChannel? materialChannel = material.FindChannel(channelKey);

            if (materialChannel == null
                || materialChannel.Value.Texture == null)
            {
                return CreateGenericTexture();
            }

            bool useSrgb = string.Compare(channelKey, CHANNEL_KEY_BASE_COLOR) == 0;

            int result = LoadImage(materialChannel.Value.Texture.PrimaryImage, useSrgb);

            return result;
        }

        private float[] InterleaveData(System.Numerics.Vector3[] positions, System.Numerics.Vector3[] normals, System.Numerics.Vector2[] uvs)
        {
            float[] result = new float[positions.Length * 8];

            for (int i = 0; i < positions.Length; i++)
            {
                int index = i * 8;

                result[index] = positions[i].X;
                result[index + 1] = positions[i].Y;
                result[index + 2] = positions[i].Z;

                result[index + 3] = normals[i].X;
                result[index + 4] = normals[i].Y;
                result[index + 5] = normals[i].Z;

                result[index + 6] = uvs[i].X;
                result[index + 7] = uvs[i].Y;
            }

            return result;
        }

        private Classes.Mesh BuildMesh(SharpGLTF.Schema2.MeshPrimitive meshPrimitive)
        {
            SharpGLTF.Schema2.MaterialChannel? metallicRoughnessChannel = meshPrimitive.Material?.FindChannel(CHANNEL_KEY_METALLIC_ROUGHNESS);
            SharpGLTF.Schema2.MaterialChannel? normalChannel = meshPrimitive.Material?.FindChannel("Normal");

            bool hasMetallicRoughnessTexture = metallicRoughnessChannel?.Texture != null;
            bool hasNormalTexture = normalChannel?.Texture != null;

            System.Numerics.Vector4 color = GetColorParameterValue(meshPrimitive);
            float metallicFactor = GetMetallicRoughnessParameterValue(meshPrimitive, "MetallicFactor");
            float roughnessFactor = GetMetallicRoughnessParameterValue(meshPrimitive, "RoughnessFactor");

            int metallicRoughnessTexture = LoadTexture(CHANNEL_KEY_METALLIC_ROUGHNESS, meshPrimitive.Material);
            int albedoTexture = LoadTexture(CHANNEL_KEY_BASE_COLOR, meshPrimitive.Material);
            int normalTexture = LoadTexture("Normal", meshPrimitive.Material);

            // calculate radians and combined rotation matrix
            float xRotationRadians = _xRotationDegrees * (MathF.PI / 180f);
            float yRotationRadians = _yRotationDegrees * (MathF.PI / 180f);
            float zRotationRadians = _zRotationDegrees * (MathF.PI / 180f);
            System.Numerics.Matrix4x4 xRotation = System.Numerics.Matrix4x4.CreateRotationX(xRotationRadians);
            System.Numerics.Matrix4x4 yRotation = System.Numerics.Matrix4x4.CreateRotationY(yRotationRadians);
            System.Numerics.Matrix4x4 zRotation = System.Numerics.Matrix4x4.CreateRotationZ(zRotationRadians);

            // scale the bounds first, then shift by the scaled center
            System.Numerics.Vector3 scaleMin = _minBounds * _scaleFactor;
            System.Numerics.Vector3 scaleMax = _maxBounds * _scaleFactor;
            System.Numerics.Vector3 scaleCenterOffset = -(_boxCenter * _scaleFactor);
            System.Numerics.Vector3[] boundingCorners = new System.Numerics.Vector3[]
            {
                new System.Numerics.Vector3(scaleMin.X, scaleMin.Y, scaleMin.Z) + scaleCenterOffset,
                new System.Numerics.Vector3(scaleMax.X, scaleMin.Y, scaleMin.Z) + scaleCenterOffset,
                new System.Numerics.Vector3(scaleMin.X, scaleMax.Y, scaleMin.Z) + scaleCenterOffset,
                new System.Numerics.Vector3(scaleMax.X, scaleMax.Y, scaleMin.Z) + scaleCenterOffset,
                new System.Numerics.Vector3(scaleMin.X, scaleMin.Y, scaleMax.Z) + scaleCenterOffset,
                new System.Numerics.Vector3(scaleMax.X, scaleMin.Y, scaleMax.Z) + scaleCenterOffset,
                new System.Numerics.Vector3(scaleMin.X, scaleMax.Y, scaleMax.Z) + scaleCenterOffset,
                new System.Numerics.Vector3(scaleMax.X, scaleMax.Y, scaleMax.Z) + scaleCenterOffset
            };         

            // spin the corners to find out where the lowest Y point falls
            System.Numerics.Matrix4x4 combinedRotation = xRotation * yRotation * zRotation;
            float lowestRotatedY = float.MaxValue;

            foreach (System.Numerics.Vector3 boundingCorner in boundingCorners)
            {
                System.Numerics.Vector3 rotatedCorner = System.Numerics.Vector3.Transform(boundingCorner, combinedRotation);

                if (rotatedCorner.Y < lowestRotatedY)
                {
                    lowestRotatedY = rotatedCorner.Y;
                }
            }

            // calculate how high we need to push the object
            float floorSnapOffset = -lowestRotatedY;

            System.Numerics.Matrix4x4 scaleMatrix = System.Numerics.Matrix4x4.CreateScale(_scaleFactor);
            System.Numerics.Matrix4x4 centerTranslation = System.Numerics.Matrix4x4.CreateTranslation(scaleCenterOffset);
            System.Numerics.Matrix4x4 floorLiftMatrix = System.Numerics.Matrix4x4.CreateTranslation(0f, floorSnapOffset, 0f);
            System.Numerics.Matrix4x4 positionsTransform = scaleMatrix * centerTranslation * combinedRotation * floorLiftMatrix;

            // push vertices to their final destination space
            System.Numerics.Vector3[] positions = meshPrimitive.GetVertexAccessor("POSITION").AsVector3Array().ToArray();

            for (int i = 0; i < positions.Length; i++)
            {
                positions[i] = System.Numerics.Vector3.Transform(positions[i], positionsTransform);

                if (positions[i].X / 2f < _x)
                {
                    _x = positions[i].X / 2f;
                }

                if (positions[i].Y / 2f < _y)
                {
                    _y = positions[i].Y / 2f;
                }

                if (positions[i].Z / 2f < _z)
                {
                    _z = positions[i].Z / 2f;
                }
            }

            // normals transform
            SharpGLTF.Schema2.Accessor normalsAccessor = meshPrimitive.GetVertexAccessor("NORMAL");
            System.Numerics.Vector3[] normals = normalsAccessor == null ? Enumerable.Repeat(new System.Numerics.Vector3(0f, 1f, 0f), positions.Length).ToArray() : normalsAccessor.AsVector3Array().ToArray();
            System.Numerics.Matrix4x4 normalsTransform = xRotation * yRotation * zRotation;

            for (int i = 0; i < normals.Length; i++)
            {
                normals[i] = System.Numerics.Vector3.TransformNormal(normals[i], normalsTransform);
            }

            // uvs transform
            SharpGLTF.Schema2.Accessor uvsAccessor = meshPrimitive.GetVertexAccessor("TEXCOORD_0");
            System.Numerics.Vector2[] uvs = uvsAccessor == null ? Enumerable.Repeat(new System.Numerics.Vector2(0f, 0f), positions.Length).ToArray() : uvsAccessor.AsVector2Array().ToArray();
            float[] vertices = InterleaveData(positions, normals, uvs);
            uint[] indices = meshPrimitive.IndexAccessor == null ? Enumerable.Range(0, positions.Length).Select(x => ((uint)x)).ToArray() : meshPrimitive.IndexAccessor.AsIndicesArray().Select(x => ((uint)x)).ToArray();
            Classes.Mesh result = new Classes.Mesh(color, metallicFactor, roughnessFactor, hasMetallicRoughnessTexture, metallicRoughnessTexture, albedoTexture, hasNormalTexture, normalTexture, vertices, indices);

            return result;
        }

        private void LoadModel()
        {
            if (_modelRoot == null)
            {
                _modelRoot = ModelRoot.Load(_path);
            }

            _minBounds = new System.Numerics.Vector3(float.MaxValue);
            _maxBounds = new System.Numerics.Vector3(float.MinValue);

            foreach (SharpGLTF.Schema2.Mesh logicalMesh in _modelRoot.LogicalMeshes)
            {
                foreach (SharpGLTF.Schema2.MeshPrimitive meshPrimitive in logicalMesh.Primitives)
                {
                    SharpGLTF.Memory.IAccessorArray<System.Numerics.Vector3> positions = meshPrimitive.GetVertexAccessor("POSITION").AsVector3Array();

                    foreach (System.Numerics.Vector3 position in positions)
                    {
                        _minBounds = System.Numerics.Vector3.Min(_minBounds, position);
                        _maxBounds = System.Numerics.Vector3.Max(_maxBounds, position);
                    }
                }
            }

            System.Numerics.Vector3 dimensions = _maxBounds - _minBounds;

            _boxCenter = (_maxBounds + _minBounds) / 2f;

            float maxDimension = Math.Max(dimensions.X, Math.Max(dimensions.Y, dimensions.Z));

            _scaleFactor = maxDimension == 0f ? 1f : _targetSize / maxDimension;

            _meshes.Clear();

            foreach (SharpGLTF.Schema2.Mesh logicalMesh in _modelRoot.LogicalMeshes)
            {
                foreach (SharpGLTF.Schema2.MeshPrimitive meshPrimitive in logicalMesh.Primitives)
                {
                    Classes.Mesh mesh = BuildMesh(meshPrimitive);

                    _meshes.Add(mesh);
                }
            }
        }

        private const string CHANNEL_KEY_BASE_COLOR = "BaseColor";
        private const string CHANNEL_KEY_METALLIC_ROUGHNESS = "MetallicRoughness";
        private string _path { get; set; }
        private float _targetSize { get; set; }
        private float _xRotationDegrees { get; set; }
        private float _yRotationDegrees { get; set; }
        private float _zRotationDegrees { get; set; }
        private ModelRoot _modelRoot { get; set; }
        private System.Numerics.Vector3 _minBounds { get; set; }
        private System.Numerics.Vector3 _maxBounds { get; set; }
        private float _scaleFactor { get; set; }
        private System.Numerics.Vector3 _boxCenter { get; set; }
        private List<Classes.Mesh> _meshes { get; set; }
        private float _x { get; set; }
        private float _y { get; set; }
        private float _z { get; set; }
    }
}