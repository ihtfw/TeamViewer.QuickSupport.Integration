using System;
using System.Linq;
using FlaUI.Core.AutomationElements;

namespace TeamViewer.QuickSupport.Integration
{
    static class Extensions
    {
        public static AutomationElement GetByNameOrDefault(this AutomationElement itemContainer, string name, params string[] alternativeNames)
        {
            var children = itemContainer.FindAllDescendants();
            return children
                .Where(c => !string.IsNullOrEmpty(c.Name))
                .FirstOrDefault(p => p.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase)
                                     || alternativeNames.Any(an => an.Equals(p.Name, StringComparison.InvariantCultureIgnoreCase)));
        }

        public static AutomationElement GetByIdOrDefault(this AutomationElement itemContainer, string id)
        {
            var children = itemContainer.FindAllDescendants(); 
            return children
                .Where(el => !string.IsNullOrEmpty(el.AutomationId))
                .FirstOrDefault(p => p.AutomationId == id);
        }
    }
}