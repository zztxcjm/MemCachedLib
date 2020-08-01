using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;

namespace MemCachedLib
{
	internal class CachedClient : IDisposable
	{
		private Socket socket;

		private BinaryFormatter serializer;

		private byte[] socketBuffer = new byte[8192];

		private ByteBuilder byteBuilder = new ByteBuilder(8192);

		private IPEndPoint ipEndPoint
		{
			get;
			set;
		}

		public CachedClient(IPEndPoint ipEndPoint)
		{
			this.ipEndPoint = ipEndPoint;
			this.serializer = new BinaryFormatter();
		}

		public T ToEntity<T>(byte[] binary)
		{
			if (binary == null || binary.Length == 0)
			{
				return default(T);
			}
			T result;
			using (MemoryStream stream = new MemoryStream())
			{
				try
				{
					stream.Write(binary, 0, binary.Length);
					stream.Position = 0L;
					result = (T)((object)this.serializer.UnsafeDeserialize(stream, null));
				}
				catch
				{
					result = default(T);
				}
			}
			return result;
		}

		public byte[] ToBinary(object value)
		{
			if (value == null)
			{
				return null;
			}
			byte[] result;
			using (MemoryStream ms = new MemoryStream())
			{
				try
				{
					this.serializer.Serialize(ms, value);
					result = ms.ToArray();
				}
				catch
				{
					result = null;
				}
			}
			return result;
		}

		public ResponseHeader Send(RequestHeader request)
		{
			return this.Sends(request).FirstOrDefault<ResponseHeader>();
		}

		public IEnumerable<ResponseHeader> Sends(RequestHeader request, int trycount = 1)
		{
			try
			{
				//重复尝试连接
				if (trycount >= Setting.TryConnectMaxCount)
				{
					throw new ConnectTimeoutException();
				}
				//非首次请求，需要等待1s在继续
				if (trycount > 1)
				{
					System.Threading.Thread.Sleep(1000);
				}

				if (this.socket == null)
				{
					this.socket = new Socket(this.ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
				}
				if (!this.socket.Connected)
				{
					this.socket.Connect(this.ipEndPoint);
				}
				this.socket.Send(request.ToByteArray());
				this.byteBuilder.Clear();
				List<ResponseHeader> responseList = new List<ResponseHeader>();
				while (true)
				{
					int readByte = this.socket.Receive(this.socketBuffer);
					this.byteBuilder.Add(this.socketBuffer, 0, readByte);
					while (this.byteBuilder.Length >= 12)
					{
						int packetLength;
						if ((packetLength = this.byteBuilder.ToInt32(8) + 24) > this.byteBuilder.Length)
						{
							break;
						}
						byte[] packetBytes = this.byteBuilder.ReadRange(packetLength);
						ResponseHeader response = new ResponseHeader(packetBytes);
						responseList.Add(response);
						if (response.OpCode != OpCodes.Stat || packetLength == 24 || response.Status != OprationStatus.No_Error)
						{
							return responseList;
						}
					}
				}
			}
			catch (SocketException)
			{
				if (this.socket != null)
				{
					try
					{
						this.socket.Close();
						this.socket.Dispose();
						this.socket = null;
					}
					catch (Exception)
					{
					}
				}
				return this.Sends(request, trycount++);
			}

		}

		public void Dispose()
		{
			if (this.socket != null)
			{
				this.socket.Shutdown(SocketShutdown.Both);
				this.socket.Dispose();
				this.socket = null;
			}
		}
	}
}
