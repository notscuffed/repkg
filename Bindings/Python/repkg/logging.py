from colorama import Style, Fore, Back


def log(text="", log_level=0):
    if log_level == -2:
        verbose(text)
    elif log_level == -1:
        debug(text)
    elif log_level == 0:
        info(text)
    elif log_level == 1:
        warn(text)
    elif log_level == 2:
        error(text)
    else:
        print(f"[{Fore.RED}{Back.RED}FAIL{Style.RESET_ALL}] {text}")


def verbose(text=""):
    print(f"[{Fore.WHITE}{Style.DIM}VRB{Style.RESET_ALL}] {Fore.WHITE}{Style.DIM}{text}{Style.RESET_ALL}")


def debug(text=""):
    print(f"[{Fore.MAGENTA}DBG{Style.RESET_ALL}] {text}")


def info(text=""):
    print(f"[INF] {text}")


def warn(text=""):
    print(f"[{Fore.YELLOW}WRN{Style.RESET_ALL}] {text}")


def error(text=""):
    print(f"[{Fore.RED}ERR{Style.RESET_ALL}] {text}")
