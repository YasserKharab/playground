using HotChocolate.PreProcessingExtensions;

namespace GraphQl.Persistence.CB
{
    public class BaseService
    {
        public string GetFieldsSelection(IParamsContext contextParams, string prefix)
        {
            var fields = "";

            foreach (var field in contextParams.AllSelectionFields)
            {
                fields += $"{prefix}.{field.SelectionMemberName} ";

                if (contextParams.AllSelectionFields.LastOrDefault() != field)
                {
                    fields += ", ";
                }
            }

            return fields;
        }
    }
}
