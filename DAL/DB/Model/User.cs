using System;

namespace DAL.DB.Model
{
    public class User
    {
        public int Id { get; set; } // Identifiant de l'utilisateur
        public string Name { get; set; } // Nom de l'utilisateur

        public bool IsDisabled { get; set; } // Indique si l'utilisateur est désactivé

        public bool IsStateFunded { get; set; } // Indique si l'étudiant est financé par l'état
        public int CardId { get; set; }  // On garde juste l'ID de la carte

    }
}
