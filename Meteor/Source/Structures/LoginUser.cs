using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/*
 * File : LoginUser
 * Author : Filipe
 * Date : 06/02/2014 19:39:38
 * Description :
 *
 */

namespace Meteor.Source
{
    public class LoginUser
    {
        public Int32 AccountId { get; set; }
        public String Username { get; set; }
        public String Password { get; set; }
        public Int32 Authority { get; set; }
        public Boolean Connected { get; set; }
    }
}
