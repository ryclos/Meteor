using Meteor.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/*
 * File : Helper
 * Author : Filipe
 * Date : 08/02/2014 20:18:58
 * Description :
 *
 */

namespace Meteor.Source
{
    public class Helper
    {
        /// <summary>
        /// Check if the character name already exists in database
        /// </summary>
        /// <param name="name">Name</param>
        /// <returns></returns>
        public static Boolean CharacterNameExists(String name)
        {
            ResultSet _rs = new ResultSet("SELECT Id FROM characters WHERE Name='{0}'", name); // TODO: create const in Query.cs
            if (_rs.Reading() == true)
            {
                _rs.Free();
                return true;
            }
            _rs.Free();
            return false;
        }

        /// <summary>
        /// Calcul la longueur d'un vecteur
        /// </summary>
        /// <param name="p">Vecteur entre 2 points</param>
        /// <returns>renvoi la longueur du vecteur</returns> 
        public static float D3DXVec3LengthSq (Position pV)
        {
            return pV.X * pV.X + pV.Y * pV.Y + pV.Z * pV.Z;
        }
        /// <summary>
        /// Retourne le produit de 2 vecteurs
        /// </summary>
        /// <param name="pV1">Premier vecteur</param>
        /// <param name="pV2">Deuxième vecteur</param>
        /// <returns></returns>
        public static float D3DXVec3Dot(Position pV1, Position pV2)
        {
            return pV1.X * pV2.X + pV1.Y * pV2.Y + pV1.Z * pV2.Z;
        }

        /// <summary>
        /// Normalise un vecteur passé en référence
        /// </summary>
        /// <param name="DeltaNorm"></param>
        /// <param name="Pos"></param>
        public static void D3DXVec3Normalize(ref Position DeltaNorm, Position Pos)
        {
            //Calcul de la longueur
            float lenght = D3DXVec3LengthSq(Pos);
            DeltaNorm.X = Pos.X / lenght;
            DeltaNorm.Y = Pos.Y / lenght;
            DeltaNorm.Z = Pos.Z / lenght;
        }
    }
}
