namespace ParserQueueRunner.Model
{
    public interface IParserWebQueue
    {
        ParserQueueElement GetNewElement();
        void SetQueueElementStatus(int ParserQueueId, int StatusId, string ErrorMessage = "");
    }
}
