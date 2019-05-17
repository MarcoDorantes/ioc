namespace nutility
{
  public class TypeClassID
  {
    public static implicit operator TypeClassID(string name) => new TypeClassID(name);

    public TypeClassID(string id)
    {
      ID = id;
    }

    public string ID { get; private set; }

    public override bool Equals(object obj)
    {
      if (obj == null)
      {
        return false;
      }

      TypeClassID other = obj as TypeClassID;
      if (other == null)
      {
        return false;
      }

      return this.ID == other.ID;
    }
    public override int GetHashCode()
    {
      return this.ID != null ? this.ID.GetHashCode() : 0;
    }
    public override string ToString()
    {
      return ID;
    }
    public bool Equals(TypeClassID other)
    {
      if (other == null)
      {
        return false;
      }

      return this.ID == other.ID;
    }
  }
}