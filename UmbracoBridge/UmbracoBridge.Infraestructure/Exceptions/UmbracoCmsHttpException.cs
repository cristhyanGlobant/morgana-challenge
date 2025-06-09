using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UmbracoBridge.Infraestructure.Exceptions;

public class UmbracoCmsHttpException : Exception
{
    public UmbracoCmsHttpException(string message) : base(message) { }
}