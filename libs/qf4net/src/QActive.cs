// -----------------------------------------------------------------------------
//                            qf4net Library
//
// Port of Samek's Quantum Framework to C#. The implementation takes the liberty
// to depart from Miro Samek's code where the specifics of desktop systems 
// (compared to embedded systems) seem to warrant a different approach.
// Please see accompanying documentation for details.
// 
// Reference:
// Practical Statecharts in C/C++; Quantum Programming for Embedded Systems
// Author: Miro Samek, Ph.D.
// http://www.quantum-leaps.com/book.htm
//
// -----------------------------------------------------------------------------
//
// Copyright (C) 2003-2004, The qf4net Team
// All rights reserved
// Lead: Rainer Hessmer, Ph.D. (rainer@hessmer.org)
// 
//
//   Redistribution and use in source and binary forms, with or without
//   modification, are permitted provided that the following conditions
//   are met:
//
//     - Redistributions of source code must retain the above copyright
//        notice, this list of conditions and the following disclaimer. 
//
//     - Neither the name of the qf4net-Team, nor the names of its contributors
//        may be used to endorse or promote products derived from this
//        software without specific prior written permission. 
//
//   THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
//   "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
//   LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS
//   FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL
//   THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT,
//   INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
//   (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
//   SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
//   HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT,
//   STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
//   ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED
//   OF THE POSSIBILITY OF SUCH DAMAGE.
// -----------------------------------------------------------------------------


using System;
using System.Diagnostics;
using System.Threading;
using System.Runtime.CompilerServices;

namespace qf4net
{
	/// <summary>
	/// The base class for Active Objects.
	/// </summary>
	public abstract class QActive : QHsm, IQActive
	{
		private IQEventQueue m_EventQueue;
		private int m_Priority;
		private Threading.IThread m_ExecutionThread;

		/// <summary>
		/// Initializes a new instance of the <see cref="QActive"/> class. 
		/// </summary>
		public QActive() : base()
		{
			m_EventQueue = EventQueueFactory.GetEventQueue();
		}

		#region IQActive Members

		/// <summary>
		/// Start the <see cref="IQActive"/> object's thread of execution. The caller needs to assign a unique
		/// priority to every <see cref="IQActive"/> object in the system. 
		/// </summary>
		/// <param name="priority">The priority associated with this <see cref="IQActive"/> object.</param>
		// TODO: Are there more flexible ways to handle the priority? Does it really need to be unique in the whole process / system?
		[MethodImpl(MethodImplOptions.Synchronized)]
		public void Start(int priority)
		{
			if (m_ExecutionThread != null)
			{
				throw new InvalidOperationException("This active object is already started. The Start method can only be invoked once.");
			}

			// Note: We use the datatype int for the priority since uint is not CLS compliant
			if (priority < 0)
			{
				throw new ArgumentException("The priority of an Active Object cannot be negative.", "priority");
			}
			m_Priority = priority;
			// TODO: Leverage the priority
			m_ExecutionThread = Threading.ThreadFactory.GetThread(0, new ThreadStart(this.DoEventLoop));
			m_ExecutionThread.Start();
		}

		/// <summary>
		/// The priority associated with this <see cref="IQActive"/> object. Once the <see cref="IQActive"/> object
		/// is started the priority is non-negative. For an <see cref="IQActive"/> object that has not yet been started
		/// the value -1 is returned as the priority.
		/// </summary>
		public int Priority { get { return m_Priority; } }

		/// <summary>
		/// Post the <see paramref="qEvent"/> directly to the <see cref="IQActive"/> object's event queue
		/// using the FIFO (First In First Out) policy. 
		/// </summary>
		/// <param name="qEvent"></param>
		public void PostFIFO(IQEvent qEvent)
		{
			m_EventQueue.EnqueueFIFO(qEvent);
		}

		/// <summary>
		/// Post the <see paramref="qEvent"/> directly to the <see cref="IQActive"/> object's event queue
		/// using the LIFO (Last In First Out) policy. 
		/// </summary>
		/// <param name="qEvent"></param>
		public void PostLIFO(IQEvent qEvent)
		{
			m_EventQueue.EnqueueLIFO(qEvent);
		}

		#endregion

		/// <summary>
		/// This method is executed on the dedicated thread of this <see cref="QActive"/> instance.
		/// </summary>
		private void DoEventLoop()
		{
			this.Init();
			// Once initialized we kick off our event loop
			try
			{
				while(true)
				{
					IQEvent qEvent = m_EventQueue.DeQueue(); // this blocks if there are no events in the queue
					//Debug.WriteLine(String.Format("Dispatching {0} on thread {1}.", qEvent.ToString(), Thread.CurrentThread.Name));
					this.Dispatch(qEvent);
					// QF.Propagate(qEvent);
				}
			}
			catch(ThreadAbortException)
			{
				// We use the method Thread.Abort() in this.Abort() to exit from the event loop
				Thread.ResetAbort();
			}

			// The QActive object ends
			m_ExecutionThread = null;
			OnExecutionAborted();
		}

		/// <summary>
		/// Aborts the execution thread. Nothing happens thereafter!
		/// </summary>
		protected void Abort()
		{
			// QF.Remove(this);
			m_ExecutionThread.Abort();
		}

		/// <summary>
		/// Allows a deriving class to react to the fact that the execution 
		/// of the active object has been aborted.
		/// </summary>
		protected virtual void OnExecutionAborted()
		{
		}
	}
}
