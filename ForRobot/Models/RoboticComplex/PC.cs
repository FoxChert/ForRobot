using System;
using System.Windows.Media.Media3D;

using ForRobot.Libr.Modeling;

namespace ForRobot.Models.RoboticComplex
{
    public class PC : SceneItem
    {
        //public Type ObjectType { get => this.GetType(); }

        public PC()
        {
            //this.Children.Add(this.GetModel());
        }

        public override void UpdateTransform(Matrix3D transform)
        {
            throw new NotImplementedException();
        }

        //public override Model3DGroup GetModel()
        //{
        //    //Vector3D pcTranslate = new Vector3D(this.X, this.Y, this.Z);
        //    Transform3DGroup transform3DGroup = Transform3DBuilder.Create().Translate(this.X, this.Y, this.Z);
        //    Model3DGroup model = ModelingService.GetPcModel(this.X, this.Y, this.Z, transform3DGroup);
        //    return model;
        //}
    }
}
