using System.Threading;

namespace Meteor.IO
{
    public class ReadWriteMutex
    {
        private ReaderWriterLockSlim m_lock = new ReaderWriterLockSlim();
        public void AcquireWriteLock()
        {
            if (!m_lock.IsWriteLockHeld)
            {
                m_lock.EnterWriteLock();
            }
        }
        public void AcquireReadLock()
        {
            m_lock.EnterReadLock();
        }
        public void ReleaseReadLock()
        {
            if (m_lock.IsReadLockHeld)
            {
                m_lock.ExitReadLock();
            }
        }
        public void ReleaseWriteLock()
        {
            if (m_lock.IsWriteLockHeld)
            {
                m_lock.ExitWriteLock();
            }
        }
    }
}
