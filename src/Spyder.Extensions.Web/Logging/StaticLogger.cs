using Microsoft.Extensions.Logging;

namespace Spyder.Extensions.Web.Logging;

public class StaticLogger
{
    #region Field

    /// <summary>
    /// Gets the logger
    /// </summary>
    private static ILogger? _logger;

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="StaticLogger"/> which injects
    /// a logger
    /// </summary>
    /// <param name="logger">The injected logger</param>
    public StaticLogger(ILogger logger)
    {
        _logger = logger;
    }

    #endregion

    #region Static Logger

    /// <summary>
    /// Returns a static instance of the ILogger
    /// </summary>
    /// <returns></returns>
    public static ILogger? Get() => _logger;

    #endregion
}