namespace PixelWallE.Core.Lexer;

public enum TokenType
{
    // Comandos básicos
    Spawn, Color, Size, DrawLine, DrawCircle, DrawRectangle, Fill,
    
    // Funciones
    GetActualX, GetActualY, GetCanvasSize, GetColorCount,
    IsBrushColor, IsBrushSize, IsCanvasColor,
    
    // Saltos Condicionales
    GoTo,       // Ej: GoTo [label] (condition) 
    Label,      // Ej: inicio_loop
    
    // Espacios
    Ignore,     // Ej: " "
    // Literales
    Number,     // Ej: 42
    String,     // Ej: "Red"
    Identifier, // Ej: miVariable
    Boolean,    // Ej: true, false
    
    // Símbolos
    LeftParen,    // (
    RightParen,   // )
    LeftBracket,  // [
    RightBracket, // ]
    Comma,        // ,
    Arrow,        // <-
    
    // Operadores aritméticos
    Plus,        // +
    Minus,       // -
    Multiply,    // *
    Divide,      // /
    Power,       // **
    Modulo,      // %
    
    // Operadores de comparación
    NotEqual,    // !=
    Equal,       // ==
    Greater,     // >
    GreaterEqual,// >=
    Less,        // <
    LessEqual,   // <=
    
    // Operadores lógicos
    And,         // &&
    Or,          // ||
    Not,         // !
    
    // Estructuras
    NewLine,     // \n
    
    // Fin de archivo
    EoF          // Fin del input
}