﻿namespace UdemyShared.Dtos
{
    public class ErrorDto
    {
        public IList<string> Errors { get; private set; }
        public bool IsShow { get; private set; }

        public ErrorDto()
        {
            Errors= new List<string>();
        }

        public ErrorDto(string error, bool isShow)
        {
            Errors = new List<string>
            {
                error
            };
            IsShow = isShow;
        }

        public ErrorDto(List<string> errors, bool isShow)
        {
            Errors = errors;           
            IsShow = isShow;
        }
    }
}
