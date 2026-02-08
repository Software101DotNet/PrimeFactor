# --- Configuration ---
TARGET := PrimeFactorC
SRC_DIR := src
BUILD_DIR := build

# Default to debug if no mode is specified
MODE ?= debug

# Compiler and standard
CXX := cc
CXXFLAGS := -Wall -Wextra -std=c23 -march=native

# Mode-specific flags
ifeq ($(MODE), release)
    CURRENT_BUILD_DIR := $(BUILD_DIR)/release
    CXXFLAGS += -O3 -DNDEBUG
else
    CURRENT_BUILD_DIR := $(BUILD_DIR)/debug
    CXXFLAGS += -g -O0
endif

# Find all .c files in SRC_DIR
SRCS := $(wildcard $(SRC_DIR)/*.c)

# Convert source files to object files in the specific build directory
OBJS := $(SRCS:$(SRC_DIR)/%.c=$(CURRENT_BUILD_DIR)/%.o)

# --- Rules ---

.PHONY: all clean help

all: $(CURRENT_BUILD_DIR)/$(TARGET)

# Link the executable
$(CURRENT_BUILD_DIR)/$(TARGET): $(OBJS)
	@echo "Linking $(MODE) build: $@"
	$(CXX) $(OBJS) -o $@

# Compile source files to object files
$(CURRENT_BUILD_DIR)/%.o: $(SRC_DIR)/%.c
	@mkdir -p $(dir $@)
	@echo "Compiling $(MODE): $<"
	$(CXX) $(CXXFLAGS) -c $< -o $@

clean:
	@echo "Cleaning up..."
	rm -rf $(BUILD_DIR)

help:
	@echo
	@echo "Usage:"
	@echo "  make               (equivalent to 'make MODE=debug')"
	@echo "  make MODE=debug"
	@echo "  make MODE=release"
	@echo "  make clean.        (clean build artifacts)"
	@echo "  make help          (display this help message)"
	@echo