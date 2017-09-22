using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace ClinicReservation.Models
{

    public static class NavigationPropertyHelper
    {
        public static void EnsureReferencesLoaded<TEntity>(this EntityEntry<TEntity> entity, bool loadcollections) where TEntity : class
        {
            if (entity == null)
                return;

            foreach (ReferenceEntry refentry in entity.References)
            {
                if (refentry.IsLoaded == false)
                    refentry.Load();
            }
            if (loadcollections == true)
            {
                foreach (CollectionEntry colentry in entity.Collections)
                {
                    if (colentry.IsLoaded == false)
                        colentry.Load();
                }
            }
        }
    }

}
