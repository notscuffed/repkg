cimport crepkg
from repkg.logging import log

_logger = None

def init():
    crepkg.set_logger(_clog)
    set_logger(log)

def set_logger(logger):
    """Sets log function

    :param logger: log function taking (log: str, log_level: int)
    """
    global _logger
    _logger = logger

cdef void _clog(const char*log, int log_level):
    global _logger
    if _logger is None:
        return
    _logger(log.decode('utf-8'), log_level)
