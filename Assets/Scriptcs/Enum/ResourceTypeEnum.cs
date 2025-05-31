public enum ResourceTypesEnum
{
    food = 1,
    wood = 2,
    gold = 4,
    stone = 8,
    allTypes = food | wood | gold | stone,
    mine = gold | stone,
}
