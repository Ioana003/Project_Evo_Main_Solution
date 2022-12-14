using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TileMap_Y12
{
    public class Camera
    {
        private Matrix transformMatrix;
        private Vector2 centre;
        private Viewport viewport;

        public Camera(Viewport inViewport)
        {
            viewport = inViewport;
        }

        public Matrix Transform
        {
            get { return transformMatrix; }
        }

		public Vector2 GetCentre()
		{
			return centre;
		}

        public void Update(Vector2 inPosition, int inXOffset, int inYOffset)
        {
            int viewPortDivider = 2;
            if (inPosition.X < viewport.Width / viewPortDivider)
            {
                centre.X = viewport.Width / viewPortDivider;
            }
            else if (inPosition.X > inXOffset - (viewport.Width / viewPortDivider))
            {
                centre.X = inXOffset - (viewport.Width / viewPortDivider);
            }
            else
            {
                centre.X = inPosition.X;
            }
            if (inPosition.Y < viewport.Height / viewPortDivider)
            {
                centre.Y = viewport.Height / viewPortDivider;
            }
            else if (inPosition.Y > inYOffset - (viewport.Height / viewPortDivider))
            {
                centre.Y = inYOffset - (viewport.Height / viewPortDivider);
            }
            else
            {
                centre.Y = inPosition.Y;
            }

            transformMatrix = Matrix.CreateTranslation(new Vector3(-centre.X + viewport.Width/viewPortDivider,
                                                                   -centre.Y + viewport.Height/viewPortDivider, 0));

			
        }

    }
}
