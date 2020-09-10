// Generated from MegaScrypt.g4 by ANTLR 4.8
import org.antlr.v4.runtime.tree.ParseTreeVisitor;

/**
 * This interface defines a complete generic visitor for a parse tree produced
 * by {@link MegaScryptParser}.
 *
 * @param <T> The return type of the visit operation. Use {@link Void} for
 * operations with no return type.
 */
public interface MegaScryptVisitor<T> extends ParseTreeVisitor<T> {
	/**
	 * Visit a parse tree produced by {@link MegaScryptParser#program}.
	 * @param ctx the parse tree
	 * @return the visitor result
	 */
	T visitProgram(MegaScryptParser.ProgramContext ctx);
	/**
	 * Visit a parse tree produced by {@link MegaScryptParser#statement}.
	 * @param ctx the parse tree
	 * @return the visitor result
	 */
	T visitStatement(MegaScryptParser.StatementContext ctx);
	/**
	 * Visit a parse tree produced by {@link MegaScryptParser#declaration}.
	 * @param ctx the parse tree
	 * @return the visitor result
	 */
	T visitDeclaration(MegaScryptParser.DeclarationContext ctx);
	/**
	 * Visit a parse tree produced by {@link MegaScryptParser#assignment}.
	 * @param ctx the parse tree
	 * @return the visitor result
	 */
	T visitAssignment(MegaScryptParser.AssignmentContext ctx);
	/**
	 * Visit a parse tree produced by {@link MegaScryptParser#id}.
	 * @param ctx the parse tree
	 * @return the visitor result
	 */
	T visitId(MegaScryptParser.IdContext ctx);
	/**
	 * Visit a parse tree produced by {@link MegaScryptParser#expression}.
	 * @param ctx the parse tree
	 * @return the visitor result
	 */
	T visitExpression(MegaScryptParser.ExpressionContext ctx);
	/**
	 * Visit a parse tree produced by {@link MegaScryptParser#number}.
	 * @param ctx the parse tree
	 * @return the visitor result
	 */
	T visitNumber(MegaScryptParser.NumberContext ctx);
}