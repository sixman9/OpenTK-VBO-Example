using System;
using OpenTK.Platform.iPhoneOS;
using MonoTouch.CoreAnimation;
using OpenTK;
using OpenTK.Graphics.ES11;
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
using MonoTouch.OpenGLES;

//See http://forums.monotouch.net/yaf_postsm1587.aspx#post1587

namespace VBOOpenTKForum
{
	public partial class EAGLView : iPhoneOSGameView
	{
		[Export("layerClass")]
		static Class LayerClass ()
		{
			return iPhoneOSGameView.GetLayerClass ();
		}
		
		[Export("initWithCoder:")]
		public EAGLView (NSCoder coder) : base(coder)
		{
			LayerRetainsBacking = false;
			LayerColorFormat = EAGLColorFormat.RGBA8;
			ContextRenderingApi = EAGLRenderingAPI.OpenGLES1;
		}
		
		protected override void ConfigureLayer (CAEAGLLayer eaglLayer)
		{
			eaglLayer.Opaque = true;
		}
		
		private float[] vertArray = { 
			20f, -20f, 20f, 
			20f, 20f, 20f, 
			-20f, 20f, 20f, 
			-20f, -20f, 20f
		};
		
		private byte[] colorArray = { 
			255, 255, 255, 255, 
			255, 255, 255, 255, 
			255, 255, 255, 255, 
			255, 255, 255, 255
		};
		
		private ushort[] indcArray = { 
			0, 1, 2, 
			2, 3, 0
		};
		
		private uint vertexBuffer = 0;
		private uint colorBuffer = 0;
		private uint indexBuffer = 0;
		
		protected override void CreateFrameBuffer ()
		{
			base.CreateFrameBuffer ();
			
			GL.Viewport (0, 0, Size.Width, Size.Height);
			GL.MatrixMode (All.Projection);
			GL.LoadIdentity ();
			
			Matrix4 perspective = Matrix4.CreatePerspectiveFieldOfView (MathHelper.DegreesToRadians (45f), ((float)Size.Width / (float)Size.Height), 0.1f, 10000f);			
			unsafe {
				GL.LoadMatrix (&perspective.Row0.X);
			}
			
			//GL.Rotate (270f, 0f, 0f, 1f);
			GL.ShadeModel (All.Flat);
			GL.ClearDepth (1f);
			GL.Enable (All.DepthTest);
			GL.DepthFunc (All.Lequal);
			GL.Hint (All.PerspectiveCorrectionHint, All.Nicest);
			GL.GenBuffers (1, ref this.vertexBuffer);
			GL.BindBuffer (All.ArrayBuffer, this.vertexBuffer);
			GL.BufferData (All.ArrayBuffer, new IntPtr ((sizeof(float) * this.vertArray.Length)), this.vertArray, All.StaticDraw);
			GL.GenBuffers (1, ref this.colorBuffer);
			GL.BindBuffer (All.ArrayBuffer, this.colorBuffer);
			GL.BufferData (All.ArrayBuffer, new IntPtr ((sizeof(byte) * this.colorArray.Length)), this.colorArray, All.StaticDraw);
			GL.GenBuffers (1, ref this.indexBuffer);
			GL.BindBuffer (All.ElementArrayBuffer, this.indexBuffer);
			GL.BufferData (All.ElementArrayBuffer, new IntPtr ((sizeof(ushort) * this.indcArray.Length)), this.indcArray, All.StaticDraw);
		}
		
		protected override void OnRenderFrame (FrameEventArgs e)
		{
			base.OnRenderFrame (e);
			
			MakeCurrent ();
			
			GL.ClearColor (0.5f, 0.5f, 0.7f, 1f);
			GL.Clear ((uint)All.ColorBufferBit);
			GL.MatrixMode (All.Modelview);
			
			Vector3 m_eye = new Vector3(0f, 0f, 250f);
			Vector3 m_target = new Vector3(0f, 0f, 0f);
			Vector3 m_up = new Vector3(0f, 1f, 0f);
			
			//Matrix4 matrix = Matrix4.LookAt (50f, 100f, 100f, 0f, 0f, 0f, 0f, 0f, 1f);
			Matrix4 matrix = Matrix4.LookAt (m_eye, m_target, m_up);
			unsafe {
				GL.LoadMatrix (&matrix.Row0.X);
			}
			
			GL.BindBuffer (All.ArrayBuffer, this.vertexBuffer);
			GL.EnableClientState (All.VertexArray);
			GL.VertexPointer (3, All.Float, 0, IntPtr.Zero);
			GL.BindBuffer (All.ArrayBuffer, this.colorBuffer);
			GL.EnableClientState (All.ColorArray);
			GL.ColorPointer (4, All.UnsignedByte, 0, IntPtr.Zero);
			GL.BindBuffer (All.ElementArrayBuffer, this.indexBuffer);
			GL.DrawElements (All.Triangles, this.indcArray.Length, All.UnsignedShort, IntPtr.Zero);
			GL.DisableClientState (All.VertexArray);
			GL.DisableClientState (All.ColorArray);
			
			SwapBuffers ();
		}
		
		protected override void DestroyFrameBuffer ()
		{
			GL.DeleteBuffers (1, ref vertexBuffer);
			GL.DeleteBuffers (1, ref colorBuffer);
			GL.DeleteBuffers (1, ref indexBuffer);
			base.DestroyFrameBuffer ();
		}
	}
}
