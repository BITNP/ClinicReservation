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
                if (!refentry.IsLoaded)
                    refentry.Load();
            }
            if (loadcollections)
            {
                foreach (CollectionEntry colentry in entity.Collections)
                {
                    if (!colentry.IsLoaded)
                        colentry.Load();
                }
            }
        }

        public static void EnsureReferencesLoaded<TEntity>(this EntityEntry<TEntity> entity, params string[] properties) where TEntity : class
        {
            if (entity == null)
                return;
            if (properties.Length <= 0)
                return;

            foreach (ReferenceEntry refentry in entity.References)
            {
                if (properties.Contains(refentry.Metadata.Name) && !refentry.IsLoaded)
                    refentry.Load();
            }
            foreach (CollectionEntry colentry in entity.Collections)
            {
                if (properties.Contains(colentry.Metadata.Name) && !colentry.IsLoaded)
                    colentry.Load();
            }
        }
    }

}
