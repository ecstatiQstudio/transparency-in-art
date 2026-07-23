using System;
using System.Numerics;
using OpenTK.Mathematics;

namespace Classes
{
    public class Camera
    {
        public Camera(float aspect)
        {
            _aspect = aspect;
            _position = new OpenTK.Mathematics.Vector3(0f, 9f, 12f);    
            _yaw = -90f;
            _pitch = -15f;

            UpdateVectors();
        }

        public OpenTK.Mathematics.Matrix4 GetProjection()
        {
            OpenTK.Mathematics.Matrix4 result = OpenTK.Mathematics.Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(60f), _aspect, 0.1f, 100f);

            return result;
        }

        public OpenTK.Mathematics.Matrix4 GetView()
        {
            OpenTK.Mathematics.Matrix4 result = Matrix4.LookAt(_position, _position + _front, OpenTK.Mathematics.Vector3.UnitY);

            return result;
        }

        public OpenTK.Mathematics.Vector3 Position { get { return _position; } }

        private void UpdateVectors()
        {
            OpenTK.Mathematics.Vector3 front;

            front.X = MathF.Cos(MathHelper.DegreesToRadians(_yaw)) * MathF.Cos(MathHelper.DegreesToRadians(_pitch));
            front.Y = MathF.Sin(MathHelper.DegreesToRadians(_pitch));
            front.Z = MathF.Sin(MathHelper.DegreesToRadians(_yaw)) * MathF.Cos(MathHelper.DegreesToRadians(_pitch));

            _front = OpenTK.Mathematics.Vector3.Normalize(front);
        }

        private OpenTK.Mathematics.Vector3 _position { get; set; }
        private float _yaw { get; set; }  
        private float _pitch {get; set; }   
        private float _aspect { get; set; }
        private OpenTK.Mathematics.Vector3 _front { get; set; }
    }
}