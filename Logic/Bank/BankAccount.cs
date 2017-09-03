using Logic.Interfaces;

namespace Logic.Bank
{
    public class BankAccount
    {
        /// <summary>
        /// ���������� �������������, ��������� � ��������������� ���������
        /// </summary>
        public string UniqueUserId { get; set; }

        /// <summary>
        /// ���-�� ����� �� �����
        /// </summary>
        public double AccountValue { get; set; }

        /// <summary>
        /// ���������� ������ �� ����������� �����
        /// </summary>
        public double DepositPercent { get; set; }

        /// <summary>
        /// ������ ������ ��������, ���������������� �� ����������
        /// </summary>
        public double TransactionComissionRate { get; set; }

        /// <summary>
        /// ������������, �������� ����������� ����
        /// </summary>
        public IExchangeUser ExchangeUser { get; set; }
    }
}