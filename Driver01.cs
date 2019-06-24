
using System;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Mogoson.Machinery
{
    [AddComponentMenu("Mogoson/Machinery/MeDriver")]
    [RequireComponent(typeof(Mechanism))]
    public class Driver01 : MonoBehaviour
    {
        #region Field and Property
        //public float velocity = 50;
        public DriveType type = DriveType.Ignore;
        //public KeyCode positive = KeyCode.P;
        //public KeyCode negative = KeyCode.N;

        protected Mechanism mechanism;
        #endregion

        #region Protected Method




        private delegate void FlushClient();//代理
        UdpClient udpClient;
        float velocity = 0.0f;

        void Start()
        {
            Initialize();
            Thread thread = new Thread(CrossThreadFlush);
            thread.IsBackground = true;
            thread.Start();
        }

        public void CrossThreadFlush()
        {
            udpClient = new UdpClient(25001);
            while (true)
            {
                ThreadFunction();
            }
        }



        private void ThreadFunction()
        {
            //IPEndPoint object will allow us to read datagrams sent from any source.
            IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
            // Blocks until a message returns on this socket from a remote host.
            Byte[] bytes = udpClient.Receive(ref RemoteIpEndPoint);
            int size = Marshal.SizeOf(bytes[0]) * bytes.Length;
            IntPtr pnt = Marshal.AllocHGlobal(size);
            Marshal.Copy(bytes, 0, pnt, bytes.Length);
            double[] managedArray2 = new double[bytes.Length / 8];
            Marshal.Copy(pnt, managedArray2, 0, bytes.Length / 8);
            this.velocity = (float)managedArray2[0];
        }






        protected virtual void Update()
        {
            DriveMechanism();
        }

        protected virtual void Initialize()
        {
            mechanism = GetComponent<Mechanism>();
        }

        protected void DriveMechanism()
        {
            mechanism.Drive(velocity, type);
        }
        #endregion
    }
}