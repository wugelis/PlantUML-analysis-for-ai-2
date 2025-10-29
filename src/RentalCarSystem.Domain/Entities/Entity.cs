namespace RentalCarSystem.Domain.Entities;

/// <summary>
/// ��H�򩳹������O�A���Ҧ������鴣�ѲΤ@���ѧO�X�M�۵��ʤ���\��
/// �ϥΪx�������T�O�ѧO�X������@ IEquatable&lt;T&gt; ����
/// </summary>
/// <typeparam name="T">�ѧO�X�������A������@ IEquatable&lt;T&gt; �����]�p Guid�Bint ���^</typeparam>
public abstract class Entity<T> where T : IEquatable<T>
{
    /// <summary>
    /// ���骺�ߤ@�ѧO�X
    /// �ϥ� protected init �T�O�u��b�غc�禡���~�����O���]�w�A�B�ȯ�]�w�@��
    /// </summary>
    public T Id { get; protected init; }
    
    /// <summary>
    /// ���O�@���غc�禡�A�T�O�u���~�����O�i�H�إ߹��
    /// �j��Ҧ����鳣�������ѧO�X
    /// </summary>
    /// <param name="id">���骺�ߤ@�ѧO�X</param>
    protected Entity(T id)
    {
        Id = id;
    }
    
    /// <summary>
    /// �мg����۵��ʤ����k
    /// ���骺�۵��ʰ���ѧO�X�A�ӫD�Ѧҩ��ݩʭ�
    /// �o�ŦX DDD�]����X�ʳ]�p�^�����骺�y�q
    /// </summary>
    /// <param name="obj">�n���������</param>
    /// <returns>
    /// �p�G��ӹ���㦳�ۦP���ѧO�X�h��^ true�A�_�h��^ false
    /// �p�G�ǤJ�����󤣬O�ۦP����������A�h��^ false
    /// </returns>
    public override bool Equals(object? obj)
    {
        // �ˬd�ǤJ����O�_���ۦP����������
        if (obj is not Entity<T> other)
            return false;
            
        // �ˬd�O�_���P�@�Ӫ���Ѧҡ]�į��u�ơ^
        if (ReferenceEquals(this, other))
            return true;
            
        // ����ѧO�X�O�_�۵�
        return Id.Equals(other.Id);
    }
    
    /// <summary>
    /// �мg����X��k�A�T�O�۵�������㦳�ۦP������X
    /// �o������b Dictionary�BHashSet �����X�������T�B�@�������n
    /// </summary>
    /// <returns>����ѧO�X������X</returns>
    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
    
    /// <summary>
    /// �۵��B��l�h���A���ѧ��[���۵��ʤ���y�k
    /// �B�z null �Ȫ����p�A�T�O������w����
    /// </summary>
    /// <param name="left">���䪺����]�i�� null�^</param>
    /// <param name="right">�k�䪺����]�i�� null�^</param>
    /// <returns>
    /// �p�G��ӹ���۵��γ��� null �h��^ true
    /// �p�G�䤤�@�Ӭ� null �ӥt�@�Ӥ��� null �h��^ false
    /// </returns>
    public static bool operator ==(Entity<T>? left, Entity<T>? right)
    {
        // �ϥ� null-conditional operator �M null-coalescing operator �B�z null ���p
        // �p�G left �� null�A�h�ˬd right �O�_�]�� null
        // �p�G left ���� null�A�h�I�s Equals ��k���
        return left?.Equals(right) ?? right is null;
    }
    
    /// <summary>
    /// ���۵��B��l�h���A���Ѫ��[�����۵�����y�k
    /// �����ϥά۵��B��l���ϦV���G
    /// </summary>
    /// <param name="left">���䪺����]�i�� null�^</param>
    /// <param name="right">�k�䪺����]�i�� null�^</param>
    /// <returns>�p�G��ӹ��餣�۵��h��^ true�A�_�h��^ false</returns>
    public static bool operator !=(Entity<T>? left, Entity<T>? right)
    {
        return !(left == right);
    }
}