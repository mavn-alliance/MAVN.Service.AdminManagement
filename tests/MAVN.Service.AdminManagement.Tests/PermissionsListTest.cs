using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Lykke.Common.ApiLibrary.Contract;
using MAVN.Service.AdminManagement.Domain.Enums;
using Xunit;

namespace MAVN.Service.AdminManagement.Tests
{
    public class PermissionsListTest
    {
        private readonly HashSet<string> _expectedPermissionLevels = new HashSet<string>
        {
            nameof(PermissionLevel.View),
            nameof(PermissionLevel.Edit),
            nameof(PermissionLevel.PartnerEdit)
        };
        
        /// <summary>
        ///     Ensures each permission level is unique and it's value has not changed.
        ///     Verifies that newly added permission levels has test cases.
        ///     Verifies if codes were removed their test cases is removed too.
        ///     If for some reasons you have modified permission levels contract,
        ///     please fix unit test cases too. This is needed to make sure you have changed permission levels knowingly.
        /// </summary>
        [Fact]
        public void PermissionLevels_WasNotModifiedAccidentally()
        {
            var currentPermissionLevels = Enum.GetNames(typeof(PermissionLevel)).ToList();

            foreach (var expectedLevel in _expectedPermissionLevels)
            {
                Assert.True(currentPermissionLevels.Contains(expectedLevel),
                    $"Permission Level: \"{expectedLevel}\" was removed! But it still have a test. If you removed it knowingly please remove it from {nameof(_expectedPermissionLevels)}.");
            }

            if (currentPermissionLevels.Count > _expectedPermissionLevels.Count)
            {
                var addedPermissionLevels = currentPermissionLevels.Except(_expectedPermissionLevels);

                foreach (var addedPermissionLevel in addedPermissionLevels)
                    Assert.True(false,
                        $"Level: \"{addedPermissionLevel}\" was added, but don't have a test. Please add it to {nameof(_expectedPermissionLevels)}.");
            }
        }
    }
}
