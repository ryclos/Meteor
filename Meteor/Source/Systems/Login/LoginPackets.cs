using Meteor.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/*
 * File : LoginPackets
 * Author : Filipe
 * Date : 06/02/2014 17:37:25
 * Description :
 *
 */

namespace Meteor.Source
{
    public partial class Client
    {
        /// <summary>
        /// Send login success
        /// </summary>
        public void SendLoginSuccess()
        {
            Packet _packet = new Packet(Header.CERTIFYRESULT);
            _packet.Add<Int32>((Int32)Error.CERT_OK);
            _packet.Add<Int32>(0);
            _packet.Add<Boolean>(false);
            this.Send(_packet);
        }
        /// <summary>
        /// Send that there is a version error between version send by client and version in server configuration
        /// </summary>
        public void SendVersionError(UInt32 realVersion )
        {
            Packet _packet = new Packet(Header.CERTIFYRESULT);
            _packet.Add<Int32>((Int32)(realVersion < Configuration.CurrentVersion ? Error.ERR_VERSION_TOO_LOW : Error.ERR_VERSION_MAINTAIN));
            this.Send(_packet);
        }
        
        /// <summary>
        /// Send login error
        /// </summary>
        /// <param name="errorCode">Error code</param>
        /// <param name="realVersion">Optional : for version error</param>
        public void SendLoginError(Error errorCode, UInt32 realVersion = 0)
        {
            Packet _packet = new Packet(Header.CERTIFYRESULT);
            _packet.Add<Int32>((Int32)errorCode);

            if (errorCode == Error.ERR_CERT_VERSION)
            {
                if (realVersion < Configuration.CurrentVersion)
                {
                    _packet.Add<Int32>((Int32)Error.ERR_VERSION_TOO_LOW);
                }
                else
                {
                    _packet.Add<Int32>((Int32)Error.ERR_VERSION_MAINTAIN);
                }
            }
            else if (errorCode == Error.ERR_CERT_BAD_USERNAME)
            {
                _packet.Add<Int32>((Int32)((UInt32)Error.CERT_CHARGE_CERTIFY_FAILED << 16 | 1));
            }
            else if (errorCode == Error.ERR_CERT_BAD_PASSWORD)
            {
                _packet.Add<Int32>((Int32)((UInt32)Error.CERT_CHARGE_CERTIFY_FAILED << 16));
            }
            else if (errorCode == Error.ERR_ACCOUNT_EXIST)
            {
                _packet.Add<Int32>((Int32)Error.ERR_ACCOUNT_EXIST);
            }
            this.Send(_packet);
        }
    }
}
