using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;

namespace Naanayam.Data
{
    public class LowerCaseElementNameConvention : IMemberMapConvention
    {
        public string Name { get { return "LowerCaseElementNameConvention"; } }

        public void Apply(BsonMemberMap map)
        {
            map.SetElementName(map.MemberName.ToLower());
        }
    }
}