using System;
using System.Windows.Media.Media3D;
using Assimp;

namespace ForRobot.Libr.Converters
{
    public static class AssimpConverter
    {
        public static Model3DGroup ConvertSceneToModel3DGroup(Scene scene)
        {
            Model3DGroup modelGroup = new Model3DGroup();

            ProcessNode(scene.RootNode, scene, modelGroup);

            return modelGroup;
        }

        private static void ProcessNode(Node node, Scene scene, Model3DGroup modelGroup)
        {

        }
    }
}
